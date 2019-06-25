#pragma strict

//this is a very basic example of how you can implement communication from javascript
//obviously, it's way better to have everything in C# or JS but we live in reality.
//When this code base is mature, I'll likely create a JS version of Camera Path for all you JS types out there
//For now - the below is the basic stuff you can access via SendMessage. It's not the sort of function you want to call 100's of times a frame though

@SerializeField
private var patha : GameObject;
@SerializeField
private var pathb : GameObject;

private var usePath : GameObject;

function Start()
{
	if(patha==null)
		return;
		
	usePath = patha;
}

function OnGUI()
{
	if(usePath==null)
		return;
		
	if(GUILayout.Button("START"))
		usePath.SendMessage("Play");
		
	if(GUILayout.Button("PAUSE"))
		usePath.SendMessage("Pause");
		
	if(GUILayout.Button("STOP"))
		usePath.SendMessage("Stop");
		
	if(GUILayout.Button("PLAY FROM HALFWAY")){
		usePath.SendMessage("Seek",0.5);
		usePath.SendMessage("Play");
	}
	
	if(GUILayout.Button("SWITCH")){
		usePath.SendMessage("Stop");
		if(usePath == patha)
			usePath = pathb;
		else
			usePath = patha;
		usePath.SendMessage("Play");
	}
	
	if(GUILayout.Button("REVERSE")){
		usePath.SendMessage("Reverse");
	}
}