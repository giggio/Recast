$('#go').click( -> 
    #$("#gotoForm").validate({ errorLabelContainer: "#errors ul", errorContainer: "#errors", wrapper: "li" });
    #$("#gotoForm").validate()
    return unless $("#gotoForm").valid()
    userName = $('#UserName').val()
    feedName = $('#FeedName').val()
    window.location = "#{baseUrl}Feed/#{userName}/#{feedName}")