(function() {

  $('#go').click(function() {
    var feedName, userName;
    if (!$("#gotoForm").valid()) {
      return;
    }
    userName = $('#UserName').val();
    feedName = $('#FeedName').val();
    return window.location = "" + baseUrl + "Feeds/" + userName + "/" + feedName;
  });

}).call(this);
