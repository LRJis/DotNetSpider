function addScript(url){
    let script = document.createElement('script');
    script.setAttribute('type','text/javascript');
    script.setAttribute('src',url);
    document.getElementsByTagName('head')[0].appendChild(script);
}
addScript("https://apps.bdimg.com/libs/jquery/2.1.4/jquery.min.js")

