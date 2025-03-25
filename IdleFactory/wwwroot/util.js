G_Language = getSavedString("Language", "EN");
function G_alert(msg){
    alert(msg);
}

function getSavedString(key, defaultValue){
    return localStorage.getItem(key) ?? defaultValue;
}

function setSavedString(key, value){
    localStorage.setItem(key, value);
}
