var _CALLERID_ = document.querySelector("_SELECTORS_");
(function() {
	if (_CALLERID_ == null) {
		return "{\"isSuccess\":false, \"errorMessage\": \"No node matches the selector _SELECTORS_\"}";
	}
	else {
		return "{\"isSuccess\":true}";
	}
})();