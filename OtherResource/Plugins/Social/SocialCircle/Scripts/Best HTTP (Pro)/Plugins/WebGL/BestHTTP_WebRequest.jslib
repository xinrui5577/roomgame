var Lib_BEST_HTTP_WebGL_HTTP_Bridge =
{
	$wr: {
		requestInstances: {},
		nextRequestId: 1
	},

	XHR_Create: function(method, url, user, passwd)
	{
		var _url = encodeURI(Pointer_stringify(url)).replace(/\+/g, '%2B');
		var _method = Pointer_stringify(method);

		console.log(wr.nextRequestId + ' XHR_Create ' + _method + ' ' + _url);

		var http = new XMLHttpRequest();

		if (user && passwd)
		{
			var u = Pointer_stringify(user);
			var p = Pointer_stringify(passwd);

			http.withCredentials = true;
			http.open(_method, _url, /*async:*/ true , u, p);
		}
		else
			http.open(_method, _url, /*async:*/ true);

		http.responseType = 'arraybuffer';

		wr.requestInstances[wr.nextRequestId] = http;
		return wr.nextRequestId++;
	},

	XHR_SetTimeout: function (request, timeout)
	{
		console.log(request + ' XHR_SetTimeout ' + timeout);

		wr.requestInstances[request].timeout = timeout;
	},

	XHR_SetRequestHeader: function (request, header, value)
	{
		var _header = Pointer_stringify(header);
		var _value = Pointer_stringify(value);

		console.log(request + ' XHR_SetRequestHeader ' + _header + ' ' + _value);

		wr.requestInstances[request].setRequestHeader(_header, _value);
	},

	XHR_SetResponseHandler: function (request, onresponse, onerror, ontimeout, onaborted)
	{
		console.log(request + ' XHR_SetResponseHandler');

		var http = wr.requestInstances[request];
		// LOAD
		http.onload = function http_onload(e) {
			console.log(request + '  - onload ' + http.status + ' ' + http.statusText);

			if (onresponse)
			{
				var response = 0;
				if (!!http.response)
					response = http.response;

				var byteArray = new Uint8Array(response);
				var buffer = _malloc(byteArray.length);
				HEAPU8.set(byteArray, buffer);

				Runtime.dynCall('viiiii', onresponse, [request, http.status, buffer, byteArray.length, 0]);

				_free(buffer);
			}
		};

		if (onerror)
		{
			http.onerror = function http_onerror(e) {
				function HandleError(err)
				{
					var buffer = _malloc(err.length + 1);
					writeStringToMemory(err, buffer);
					Runtime.dynCall('vii', onerror, [request, buffer]);
					_free(buffer);
				}

				if (e.error)
					HandleError(e.error);
				else
					HandleError("Unknown Error! Maybe a CORS porblem?");
			};	
		}

		if (ontimeout)
			http.ontimeout = function http_onerror(e) {
				Runtime.dynCall('vi', ontimeout, [request]);
			};	

		if (onaborted)
			http.onabort = function http_onerror(e) {
				Runtime.dynCall('vi', onaborted, [request]);
			};	
	},

	XHR_SetProgressHandler: function (request, onprogress, onuploadprogress)
	{
		console.log(request + ' XHR_SetProgressHandler');

		var http = wr.requestInstances[request];
		if (http)
		{
			if (onprogress)
				http.onprogress = function http_onprogress(e) {
					console.log(request + ' XHR_SetProgressHandler - onProgress ' + e.loaded + ' ' + e.total);

					if (e.lengthComputable)
						Runtime.dynCall('viii', onprogress, [request, e.loaded, e.total]);
				};

			if (onuploadprogress)
				http.upload.addEventListener("progress", function http_onprogress(e) {
					console.log(request + ' XHR_SetProgressHandler - onUploadProgress ' + e.loaded + ' ' + e.total);
					if (e.lengthComputable) 
						Runtime.dynCall('viii', onuploadprogress, [request, e.loaded, e.total]);
				}, true);
		}
	},

	XHR_Send: function (request, ptr, length)
	{
		console.log(request + ' XHR_Send ' + ptr + ' ' + length);

		var http = wr.requestInstances[request];

		try {
			if (length > 0)
				http.send(HEAPU8.subarray(ptr, ptr+length));
			else
				http.send();
		}
		catch(e) {
			console.error(request + ' ' + e.name + ": " + e.message);
		}
	},

	XHR_GetResponseHeaders: function(request, callback) 
	{
		console.log(request + ' XHR_GetResponseHeaders');

		var headers = wr.requestInstances[request].getAllResponseHeaders();
		console.log('  "' + headers + '"');

		var byteArray = new Uint8Array(headers.length);
		for(var i=0,j=headers.length;i<j;++i){
			byteArray[i]=headers.charCodeAt(i);
		}

		var buffer = _malloc(byteArray.length);
		HEAPU8.set(byteArray, buffer);

		Runtime.dynCall('viii', callback, [request, buffer, byteArray.length]);

		_free(buffer);
	},

	XHR_GetStatusLine: function(request, callback) 
	{
		console.log(request + ' XHR_GetStatusLine');

		var status = "HTTP/1.1 " + wr.requestInstances[request].status + " " + wr.requestInstances[request].statusText;

		console.log(status);

		var byteArray = new Uint8Array(status.length);
		for(var i=0,j=status.length;i<j;++i){
			byteArray[i]=status.charCodeAt(i);
		}
		var buffer = _malloc(byteArray.length);
		HEAPU8.set(byteArray, buffer);

		Runtime.dynCall('viii', callback, [request, buffer, byteArray.length]);

		_free(buffer);
	},

	XHR_Abort: function (request)
	{
		console.log(request + ' XHR_Abort');

		wr.requestInstances[request].abort();
	},

	XHR_Release: function (request)
	{
		console.log(request + ' XHR_Release');

		delete wr.requestInstances[request];
	}
};

autoAddDeps(Lib_BEST_HTTP_WebGL_HTTP_Bridge, '$wr');
mergeInto(LibraryManager.library, Lib_BEST_HTTP_WebGL_HTTP_Bridge);
