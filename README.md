# UnityMechanicalTurk
This project is a Unity plugin that assists the recording of a side-by-side comparison of two competing features within a user-controlled environment.

Our API can be found [here](https://shanemcdermott.github.io/UnityMechanicalTurk/index.html).

Our feature comparison tool can be accessed from the editor menu bar. Selecting GameObject->Camera Controller->Standard will create a GameObject with a CameraController component and two cameras prepared for split-screen rendering. GameObject->Camera Controller->Generator will create a specialized CameraController that focuses on two GenerationControllers. When the end of a ViewSequence has been reached, the GenerationController seeds are incremented and their algorithms are run again. After the algorithms have finished, the view sequence starts over.
Additional CameraViewTargets can be created either through the CameraController’s Inspector tab or through the menu bar (GameObject->Camera Controller->View Target). 

Additional camera splits can be added by clicking “+” in the CameraController’s inspector panel. 
