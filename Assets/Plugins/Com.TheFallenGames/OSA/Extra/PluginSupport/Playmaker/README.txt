File updated on 11.03.2020
Check the latest version at https://thefallengames.com/unityassetstore/optimizedscrollviewadapter/playmaker/README.txt


Playmaker support only works with Unity 2017.1.0f3 and up + Playmaker 1.9.0 an up

Instructions:

1.   Back up your project!

2.   Make sure the original Playmaker asset is imported/installed (it's not included, obviously. Here's their asset link: https://assetstore.unity.com/packages/tools/visual-scripting/playmaker-368)
	 It should be AT LEAST version 1.9.0

3.   (Optional, if you want to use XML) Import their DataMaker add-on (https://hutonggames.fogbugz.com/default.asp?W1133). It doesn't work on Windows Phone!

4.   Extract the PlaymakerSupport.zip somewhere OUTSIDE the project. In some unity versions, the packages inside won't be double-clickable, so you need to import them through drag'n'drop or toolbar

5.   This step should be done for OSA 4.3 and up, since the utilities and demos were separated in their own package. Import Utilities and then Demos packages (in this order) found under "/Extra"

6.   Import the PMSupport-Unity2017-1-f3.unitypackage

7.   For Unity 2017.3 and up, you'll probably get an error because OSA can't see the Playmaker code. This is normal.
	 A. If you don't want to use ASMDEFs or just prefer a quicker set up, delete all *.asmdef files from OSA's folder.
	 B. If you know how to set up ASMDEFs (https://docs.unity3d.com/2017.4/Documentation/Manual/ScriptCompilationAssemblyDefinitionFiles.html), 
	    Open /Scripts/OSA.asmdef and add a reference to the Playmaker's asmdef file(s). Note that (at the moment of writing, 11.03.2020) Playmaker doesn't use .asmdef files
	    by default (probably for compatibility reasons), so you'd have to manage it yourself.

8.   Wait for scripts to compile. A compile directive will be added automatically as OSA_PLAYMAKER

9.   (Optional, if #3 was also done) Import the DataMakerXMLTemplates-Unity2017-1-f3.unitypackage. Wait for scripts to compile.

10.  Right-click on an UI element in the scene and choose UI->OSA. Follow the instructions in the wizard to set up some basic examples to get you started.

11.  See the youtube tutorial linked in the manual (See "Playmaker support video" at the beginning of the doc under "External links"): https://docs.google.com/document/d/1exc3hz9cER9fKx2m0rXxTG0-vMxEGdFrd1NYdDJuATk

12.  See what you can do with OSA through Playmaker: https://docs.google.com/document/d/1exc3hz9cER9fKx2m0rXxTG0-vMxEGdFrd1NYdDJuATk/view#bookmark=id.d2r5nao1bj3j


