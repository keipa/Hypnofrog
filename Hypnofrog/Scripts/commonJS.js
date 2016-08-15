function getPhotoUrl(userid, siteid, value) {
    return "http://localhost:61065/Home/UpdateRating?userid=" + userid+ "&siteid="+siteid+"&value=" + value;
};

function getDummyImage(backcolor, fontcolor, title) {
    return "http://dummyimage.com/400x150/"+backcolor+"/"+fontcolor+"&text="+title;
};