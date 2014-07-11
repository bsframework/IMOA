function start(){ //主目录的关联函数已经对应了这里的两个函数
    console.log("Hello Start");
    return "Hello Start";
}
 
function upload(){
    console.log("Hello Upload");
    return "Hello Upload";
}
 
exports.start = start;
exports.upload = upload;