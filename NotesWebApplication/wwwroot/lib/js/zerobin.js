/**
 * Converts an associative array to an encoded string
 * for appending to the anchor.
 *
 * @param object associative_array Object to be serialized
 * @return string
 */
function hashToParameterString(associativeArray)
{
  var parameterString = ""
  for (key in associativeArray)
  {
    if( parameterString === "" )
    {
      parameterString = encodeURIComponent(key);
      parameterString += "=" + encodeURIComponent(associativeArray[key]);
    } else {
      parameterString += "&" + encodeURIComponent(key);
      parameterString += "=" + encodeURIComponent(associativeArray[key]);
    }
  }
  //padding for URL shorteners
  parameterString += "&p=p";
  
  return parameterString;
}

/**
 * Converts a string to an associative array.
 *
 * @param string parameter_string String containing parameters
 * @return object
 */
function parameterStringToHash(parameterString)
{
  var parameterHash = {};
  var parameterArray = parameterString.split("&");
  for (var i = 0; i < parameterArray.length; i++) {
    //var currentParamterString = decodeURIComponent(parameterArray[i]);
    var pair = parameterArray[i].split("=");
    var key = decodeURIComponent(pair[0]);
    var value = decodeURIComponent(pair[1]);
    parameterHash[key] = value;
  }
  
  return parameterHash;
}

/**
 * Get an associative array of the parameters found in the anchor
 *
 * @return object
 **/
function getParameterHash()
{
  var hashIndex = window.location.href.indexOf("#");
  if (hashIndex >= 0) {
    return parameterStringToHash(window.location.href.substring(hashIndex + 1));
  } else {
    return {};
  } 
}

/**
 * Compress a message (deflate compression). Returns base64 encoded data.
 *
 * @param string message
 * @return base64 string data
 */
function compress(message) {
    return Base64.toBase64( RawDeflate.deflate( Base64.utob(message) ) );
}

/**
 * Decompress a message compressed with compress().
 */
function decompress(data) {
    return Base64.btou( RawDeflate.inflate( Base64.fromBase64(data) ) );
}

/**
 * Compress, then encrypt message with key.
 *
 * @param string key
 * @param string message
 * @return encrypted string data
 */
function zeroCipher(key, message) {
    return sjcl.encrypt(key,compress(message));
}
/**
 *  Decrypt message with key, then decompress.
 *
 *  @param key
 *  @param encrypted string data
 *  @return string readable message
 */
function zeroDecipher(key, data) {
    return decompress(sjcl.decrypt(key,data));
}

/**
 * @return the current script location (without search or hash part of the URL).
 *   eg. http://server.com/zero/?aaaa#bbbb --> http://server.com/zero/
 */
function scriptLocation() {
  var scriptLocation = window.location.href.substring(0,window.location.href.length
    - window.location.search.length - window.location.hash.length);
  var hashIndex = scriptLocation.indexOf("#");
  if (hashIndex !== -1) {
    scriptLocation = scriptLocation.substring(0, hashIndex)
  }
  return scriptLocation
}

/**
 * @return the paste unique identifier from the URL
 *   eg. 'c05354954c49a487'
 */
function pasteID() {
    return window.location.search.substring(1);
}

function htmlEntities(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}
/**
 * Set text of a DOM element (required for IE)
 * This is equivalent to element.text(text)
 * @param object element : a DOM element.
 * @param string text : the text to enter.
 */
function setElementText(element, text) {
    // For IE<10.
    if ($('div#oldienotice').is(":visible")) {
        // IE<10 does not support white-space:pre-wrap; so we have to do this BIG UGLY STINKING THING.
        var html = htmlEntities(text).replace(/\n/ig,"\r\n<br>");
        element.html('<pre>'+html+'</pre>');
    }
    // for other (sane) browsers:
    else {
        element.text(text);
    }
}

/** Apply syntax coloring to clear text area.
  */
function applySyntaxColoring()
{
    if ($('div#cleartext').html().substring(0,11) != '<pre><code>')
    {
        // highlight.js expects code to be surrounded by <pre><code>
        $('div#cleartext').html('<pre><code>'+ $('div#cleartext').html()+'</code></pre>');
    }
    hljs.highlightBlock(document.getElementById('cleartext'));
    $('div#cleartext').css('padding','0'); // Remove white padding around code box.
}


/**
 * Show decrypted text in the display area, including discussion (if open)
 *
 * @param string key : decryption key
 * @param array comments : Array of messages to display (items = array with keys ('data','meta')
 */
function displayMessages(key, comments) {
    try { // Try to decrypt the paste.
        var cleartext = zeroDecipher(key, comments.data);
    } catch(err) {
        $('div#cleartext').hide();
        $('button#clonebutton').hide();
        showError('Could not decrypt data (Wrong key ?)');
        return;
    }
    setElementText($('div#cleartext'), cleartext);
    urls2links($('div#cleartext')); // Convert URLs to clickable links.

    // comments[0] is the paste itself.

    if (comments.syntaxHighlighting) applySyntaxColoring();

    // Display paste expiration.
    //if (comments[0].meta.expire_date) $('div#remainingtime').removeClass('foryoureyesonly').text('This document will expire in '+secondsToHuman(comments[0].meta.remaining_time)+'.').show();
    if (comments.burnAfterReading) {
        $('div#burnWarning').append('<p class="mb-0">For your eyes only.  Don\'t close this window, this message can\'t be displayed again.</p>').show();
        $('button#clonebutton').hide(); // Discourage cloning (as it can't really be prevented).
    }
}


/**
 *  Send a new paste to server
 */
function send_data() {
    // Do not send if no data.
    if ($('textarea#message').val().length == 0) {
        return;
    }

    // If sjcl has not collected enough entropy yet, display a message.
    if (!sjcl.random.isReady())
    {
        showStatus('Sending paste (Please move your mouse for more entropy)...', spin=true);
        sjcl.random.addEventListener('seeded', function(){ send_data(); }); 
        return; 
    }
    
    showStatus('Sending paste...', spin=true);

    var randomkey = sjcl.codec.base64.fromBits(sjcl.random.randomWords(8, 0), 0);
    var cipherdata = zeroCipher(randomkey, $('textarea#message').val());
    var data_to_send = { data:           cipherdata,
                         destroying: $('input#burnafterreading').is(':checked') ? true : false,
                         syntaxhighlighting: $('input#syntaxcoloring').is(':checked') ? true : false
                       };
    $.post(scriptLocation() + 'api/notes/add', data_to_send, 'json')
        .fail(function () {
            showError('Data could not be sent (server error or not responding).');
        })
        .done(function (data) {
            if (data.status == 0) {
                stateExistingPaste();
                var url = scriptLocation() + "?id=" + encodeURIComponent(data.id) + '&key=' + encodeURIComponent(randomkey);
                var deleteUrl = scriptLocation() + "?id=" + encodeURIComponent(data.id) + '&deletetoken=' + encodeURIComponent(data.deleteToken);
                showStatus('');

                $('div#pastelink').html('Your note url <a id="pasteurl" href="' + url + '">' + url + '</a>');
                $('div#deletelink').html('<a href="' + deleteUrl + '">Click here to delete</a>');
                $('div#pasteresult').show();
                selectText('pasteurl'); // We pre-select the link so that the user only has to CTRL+C the link.

                setElementText($('div#cleartext'), $('textarea#message').val());
                urls2links($('div#cleartext'));

                // FIXME: Add option to remove syntax highlighting ?
                if ($('input#syntaxcoloring').is(':checked')) applySyntaxColoring();

                showStatus('');
            }
            else if (data.status != 1) {
                showError('Error: '+data.message);
            }
            else {
                showError('Error.');
            }
        });
}


/** Text range selection.
 *  From: http://stackoverflow.com/questions/985272/jquery-selecting-text-in-an-element-akin-to-highlighting-with-your-mouse
 *  @param string element : Indentifier of the element to select (id="").
 */
function selectText(element) {
    var doc = document
        , text = doc.getElementById(element)
        , range, selection
    ;    
    if (doc.body.createTextRange) { //ms
        range = doc.body.createTextRange();
        range.moveToElementText(text);
        range.select();
    } else if (window.getSelection) { //all others
        selection = window.getSelection();        
        range = doc.createRange();
        range.selectNodeContents(text);
        selection.removeAllRanges();
        selection.addRange(range);
    }
}
/**
 * Put the screen in "New paste" mode.
 */
function stateNewPaste() {
    $('button#sendbutton').show();
    $('button#clonebutton').hide();
    $('button#rawtextbutton').hide();
    $('div#expiration').show();
    $('div#remainingtime').hide();
    $('div#switchbar').show();
    $('button#newbutton').show();
    $('div#pasteresult').hide();
    $('textarea#message').text('');
    $('div#messageContainer').show();
    $('div#cleartextContainer').hide();
    $('div#message').focus();
}

/**
 * Put the screen in "Existing paste" mode.
 */
function stateExistingPaste() {
    $('button#sendbutton').hide();

    // No "clone" for IE<10.
    if ($('div#oldienotice').is(":visible")) {
        $('button#clonebutton').hide();
    }
    else {
        $('button#clonebutton').show();
    }
    $('button#rawtextbutton').show();

    $('div#expiration').hide();
    $('div#switchbar').hide();
    $('div#opendisc').hide();  
    $('button#newbutton').show();
    $('div#pasteresult').hide();
    $('div#messageContainer').hide();
    $('div#cleartextContainer').show();
}

/** Return raw text
  */
function rawText()
{
    var paste = $('div#cleartext').html();
    var newDoc = document.open('text/html', 'replace');
    newDoc.write('<pre>'+paste+'</pre>');
    newDoc.close();
}

/**
 * Clone the current paste.
 */
function clonePaste() {
    stateNewPaste();
    
    //Erase the id and the key in url
    history.replaceState(document.title, document.title, scriptLocation());
    
    showStatus('');
    $('textarea#message').text($('div#cleartext').text());
}

/**
 * Create a new paste.
 */
function newPaste() {
    stateNewPaste();
    showStatus('');
    $('textarea#message').text('');
}

/**
 * Display an error message
 * (We use the same function for paste and reply to comments)
 */
function showError(message) {
    $('div#status').addClass('errorMessage').text(message);
    $('div#replystatus').addClass('errorMessage').text(message);
}

/**
 * Display status
 * (We use the same function for paste and reply to comments)
 *
 * @param string message : text to display
 * @param boolean spin (optional) : tell if the "spinning" animation should be displayed.
 */
function showStatus(message, spin) {
    $('div#replystatus').removeClass('errorMessage');
    $('div#replystatus').text(message);
    if (!message) {
        $('div#status').html('&nbsp;');
        return;
    }
    if (message == '') {
        $('div#status').html('&nbsp;');
        return;
    }
    $('div#status').removeClass('errorMessage');
    $('div#status').append(message).show;
}

/**
 * Convert URLs to clickable links.
 * URLs to handle:
 * <code>
 *     magnet:?xt.1=urn:sha1:YNCKHTQCWBTRNJIV4WNAE52SJUQCZO5C&xt.2=urn:sha1:TXGCZQTH26NL6OUQAJJPFALHG2LTGBC7
 *     http://localhost:8800/zero/?6f09182b8ea51997#WtLEUO5Epj9UHAV9JFs+6pUQZp13TuspAUjnF+iM+dM=
 *     http://user:password@localhost:8800/zero/?6f09182b8ea51997#WtLEUO5Epj9UHAV9JFs+6pUQZp13TuspAUjnF+iM+dM=
 * </code>
 *
 * @param object element : a jQuery DOM element.
 * @FIXME: add ppa & apt links.
 */
function urls2links(element) {
    var re = /((http|https|ftp):\/\/[\w?=&.\/-;#@~%+-]+(?![\w\s?&.\/;#~%"=-]*>))/ig;
    element.html(element.html().replace(re,'<a href="$1" rel="nofollow">$1</a>'));
    var re = /((magnet):[\w?=&.\/-;#@~%+-]+)/ig;
    element.html(element.html().replace(re,'<a href="$1">$1</a>'));
}

/**
 * Return the deciphering key stored in anchor part of the URL
 */
function pageKey() {
    var key = window.location.hash.substring(1);  // Get key

    // Some stupid web 2.0 services and redirectors add data AFTER the anchor
    // (such as &utm_source=...).
    // We will strip any additional data.

    // First, strip everything after the equal sign (=) which signals end of base64 string.
    i = key.indexOf('='); if (i>-1) { key = key.substring(0,i+1); }

    // If the equal sign was not present, some parameters may remain:
    i = key.indexOf('&'); if (i>-1) { key = key.substring(0,i); }

    // Then add trailing equal sign if it's missing
    if (key.charAt(key.length-1)!=='=') key+='=';

    return key;
}

$(function() {

    // Display status returned by php code if any (eg. Paste was properly deleted.)
    if ($('div#status').text().length > 0) {
        showStatus($('div#status').text(),false);
        return;
    }

    $('div#status').html('&nbsp;'); // Keep line height even if content empty.



    var url_string = window.location.href;
    var url = new URL(url_string);
    var id = url.searchParams.get("id");
    var key = url.searchParams.get("key");
    var deletetoken = url.searchParams.get("deletetoken");
    if (deletetoken != null) {

        $.get(window.location.origin + "/api/notes/delete?id=" + encodeURIComponent(id) + "&deletetoken=" + encodeURIComponent(deletetoken), function () {
            newPaste();
            return;
        });
    }
    else
    if (id != null) {
        
        $.get(window.location.origin + "/api/notes/read?id=" + id, function (data) {
            if (data.status == 0) {
                stateExistingPaste();

                displayMessages(key, data);
            } else
                newPaste();
        });
    }
    else
        newPaste(); 
});
