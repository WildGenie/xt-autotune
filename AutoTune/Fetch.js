"use strict";
const scriptError = "script-error";
const scriptTimeout = "script-timeout";

function iFrameDoc() {
  return document.getElementById('container').contentWindow.document;
}

function schedule(fun, delay, retries, retry) {
  if (retry == retries)
    return callback.accept(scriptTimeout);
  else
    setTimeout(function () { fun(delay, retries, retry + 1); }, delay);
}
