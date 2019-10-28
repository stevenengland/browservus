var _CALLERID_ = document.getElementById("_ID_");
(function() {
	if (_CALLERID_ == null) {
		return "{\"isSuccess\":false, \"errorMessage\": \"No node matches the ID _ID_\"}";
	}
	else {
		return "{\"isSuccess\":true}";
	}
})();