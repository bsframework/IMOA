function route(handle,pathname){ //路由功能，接收两个参数，关联函数和路径
	console.log("About to route a request for " + pathname); 
	if (typeof handle[pathname] === 'function') { //如果有，那么直接执行关联函数
		return handle[pathname]();
	} 
	else { 
		console.log("No request handler found for " + pathname);
                   return "404 Not found"; // 没有返回404
		} 
 
}
 
exports.route = route;