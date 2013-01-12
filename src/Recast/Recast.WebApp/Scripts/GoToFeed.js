(function() {

  $('#go').click(function() {
    var feedName, userName;
    if (!$("#gotoForm").valid()) {
      return;
    }
    userName = $('#UserName').val();
    feedName = $('#FeedName').val();
    return window.location = "" + baseUrl + "Feed/" + userName + "/" + feedName;
  });

}).call(this);
