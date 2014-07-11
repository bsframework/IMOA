var server = require('./server'); //获得server.js模块
var router = require('./router'); //获得router.js模块
var requestHandlers = require("./requestHandlers"); //获得router.js模块
 
var handle = {}; //可以看作是关联函数，分析URL后，将不同的请求给与对应的处理
handle["/"] = requestHandlers.start; 
handle["/start"] = requestHandlers.start; 
handle["/upload"] = requestHandlers.upload;
 
server.start(router.route,handle); //两个参数分别是路由和这个关联函数，