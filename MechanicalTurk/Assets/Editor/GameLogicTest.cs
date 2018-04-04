using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;


public class GameLogicTest {

	[Test]
	public void VerifySceneContentsFail()
	{
		var go = GameObject.Find("cube");
		Assert.IsNotNull(go, "No cube object found in scene.");
	}

	[Test]
	public void VerifySceneContentsPass()
	{
		var go = GameObject.Find("Map Generator");
		Assert.IsNotNull(go, "No Map Generator object found in scene.");
	}

	[UnityTest]
	public IEnumerator SceneGeneratesMap()
	{
		//SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
		yield return null;
		var map = GameObject.Find("Map Generator");
		var mapGen = map.GetComponent<MapGenerator>();
		Assert.True(mapGen != null);
	}

	[Test]
	[UnityPlatform(exclude = new[] {RuntimePlatform.OSXEditor })]
	public void SceneHasCameraExcludeMac()
	{
		var go = GameObject.Find("Camera");
		Assert.IsNotNull(go, "No camera object found in scene.");
	}

	[Test]
	[UnityPlatform(exclude = new[] {RuntimePlatform.WindowsEditor })]
	public void SceneHasCameraWindows()
	{
		var go = GameObject.Find("Camera");
		Assert.IsNotNull(go, "No camera object found in scene.");
	}
}
