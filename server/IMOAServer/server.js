var http = require('http'); 
var url = require('url');
 
function start(route,handle){ 
http.createServer(function(req, res){
    var pathname = url.parse(req.url).pathname;
    console.log("Request for " + pathname + " received."); 
     res.writeHead(200, {'Content-Type': 'text/html'});
    var content = route(handle,pathname);
    res.write(content);
    res.end();
}).listen(3000);
console.log("HTTP server is listening at port 3000.");
}
 
exports.start = start; 