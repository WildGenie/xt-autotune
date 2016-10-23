"use strict";

// The fetch application callback expects either one of these 2 values or an actual download link.
const scriptError = "script-error";
const scriptTimeout = "script-timeout";

// Returns the document element in the container iframe.
function iFrameDoc() {
  return document.getElementById('container').contentWindow.document;
}

// Run next retry of the given function in (delay) millis.
function schedule(fun, delay, retries, retry) {
  if (retry == retries)
    return callback.accept(scriptTimeout);
  else
    setTimeout(function () { fun(delay, retries, retry + 1); }, delay);
}
