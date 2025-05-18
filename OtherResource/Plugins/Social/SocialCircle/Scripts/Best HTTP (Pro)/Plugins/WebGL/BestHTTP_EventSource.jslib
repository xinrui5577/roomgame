var Lib_BEST_HTTP_WebGL_ES_Bridge =
{
	$es: {
		eventSourceInstances: {},
		nextInstanceId : 1,

		Set : function(event) {
			es.eventSourceInstances[es.nextInstanceId] = event;
			return es.nextInstanceId++;
		},

		Get : function(id) {
			return es.eventSourceInstances[id];
		},

		Remove: function(id) {
			delete es.eventSourceInstances[id];
		},

		_callOnError: function(errCallback, id, reason)
		{
			if (reason)
			{
				var buffer = _malloc(reason.length + 1);
				writeStringToMemory(reason, buffer);
				Runtime.dynCall('vii', errCallback, [id, buffer]);
				_free(buffer);
			}
			else
				Runtime.dynCall('vii', errCallback, [id, 0]);
		}
	},

	ES_Create: function(urlPtr, withCredentials, onOpen, onMessage, onError)
	{
		var url = encodeURI(Pointer_stringify(urlPtr)).replace(/\+/g, '%2B');

		var event = {
			onError: onError
		};

		var id = ws.nextInstanceId;

		console.log(id + ' ES_Create(' + url + ', ' + withCredentials + ')');

		event.eventImpl = new EventSource(url, { withCredentials: withCredentials != 0 ? true : false } );

		event.eventImpl.onopen = function() {
			console.log(id + ' ES_Create - onOpen');

			Runtime.dynCall('vi', onOpen, [id]);
		};

		event.eventImpl.onmessage = function(e) {

			function AllocString(str) {
				if (str != undefined)
				{
					var buff = _malloc(str.length + 1);
					writeStringToMemory(str, buff);
					return buff;
				}

				return 0;
			}

			var eventBuffer = AllocString(e.event);
			var dataBuffer = AllocString(e.data);
			var idBuffer = AllocString(e.id);

			Runtime.dynCall('viiiii', onMessage, [id, eventBuffer, dataBuffer, idBuffer, e.retry]);

			if (eventBuffer != 0)
				_free(eventBuffer);

			if (dataBuffer != 0)
				_free(dataBuffer);

			if (idBuffer != 0)
				_free(idBuffer);
		};

		event.eventImpl.onerror = function(e) {
			console.log(id + ' ES_Create - onError');

			es._callOnError(onError, id, "Unknown Error!");
		};

		return es.Set(event);
	},

	ES_Close: function(id)
	{
		console.log(id + ' ES_Close');

		var event = es.Get(id);

		try
		{
			event.close();
		}
		catch(e) {
			es._callOnError(event.onError, id, ' ' + e.name + ': ' + e.message);
		}
	},

	ES_Release: function(id)
	{
		console.log(id + ' ES_Release');

		es.Remove(id);
	}
};

autoAddDeps(Lib_BEST_HTTP_WebGL_ES_Bridge, '$es');
mergeInto(LibraryManager.library, Lib_BEST_HTTP_WebGL_ES_Bridge);
