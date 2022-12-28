# Made with Ren'Py Maker Lite
# By Alan Baylis
# https://www.renpymaker.com

define Cindy_ = Character("Cindy", image="cindy", color="#ff0700")
define Michael_ = Character("Michael", image="michael", color="#f5ff00")
define Lucy_ = Character("Lucy", image="lucy", color="#ff00d6")

image cindy happy = "cindy_happy.png"
image cindy hips = "cindy_hips.png"
image cindy neutral = "cindy_neutral.png"
image cindy chuckle = "cindy_chuckle.png"
image cindy smile = "cindy_smile.png"
image cindy reserved = "cindy_reserved.png"
image cindy direct = "cindy_direct.png"
image cindy bye = "cindy_bye.png"

image side cindy = "side_cindy.png"
image side michael = "side_michael.png"
image side lucy = "side_lucy.png"

label start:

# Press the Play button in Unity

    scene background

    show cindy happy at center

    Cindy_ "Hello, and welcome to Ren'Py Maker. Please click on me to advance the tutorial."

    hide cindy happy

    show cindy hips at center

    Cindy_ "This is a quick overview of what this software is and how to get started. "

    Cindy_ "Ren'Py Maker is a utility for creating the script files used by Ren'Py to then create a visual novel. "

    Cindy_ "The main window is called a Graph Editor and the contents of this window are called Nodes. You will notice that all of the nodes are connected together to create the graph."

    Cindy_ "To add new nodes to the graph you can right click on the Graph window to open the context menu and hover over the Nodes item. I don't recommend changing the nodes or adding new nodes while the tutorial is running in Unity though. You can do that after. "

    Cindy_ "The first node in the graph should always be the Start node. This is where your story begins."

    Cindy_ "The only nodes that are not connected to the rest of the graph are called Character nodes. Which define the characters that will appear in your story."

    hide cindy hips

    show cindy neutral at center

    Cindy_ "See how the focus of the current node is following our conversation."

    Cindy_ "While this tutorial is playing you can right click on the Graph window and drag the mouse to scroll the view. Pressing the middle mouse button does the same thing if you prefer to use that button."

    Cindy_ "When you are mousing over the Graph window you can also use the scroll wheel to zoom the view in and out."

    Cindy_ "You can connect two nodes together by left clicking and dragging the mouse on the Exit port of one node to the Entry port of another and then releasing the mouse button."

    Cindy_ "And you can disconnect two nodes by left clicking the mouse on the Entry port where they are connected and then dragging the mouse to somewhere over the background before releasing the mouse button."

    hide cindy neutral

    show cindy hips at center

    Cindy_ "No doubt you've already noticed that the Dialogue nodes contain the text and character names to be used in the story."

    Cindy_ "And you might have already guessed how we change the image being displayed. Namely by using the Show and Hide nodes. "

    Cindy_ "I think it is time that we changed this drab background to something a little more lively."

    Cindy_ "We can do this with a Scene node. This will clear the background and any visible images and replace them with a new background image."

    scene restaurant

    show cindy happy at center

    Cindy_ "That's better. But now I look a little out of place. Excuse me while I slip into something a bit more comfortable."

    hide cindy happy

    show cindy chuckle at center

    Cindy_ "Wonderful. I love this restaurant, and check out that view!"

menu:

    Cindy_ "Would you like to listen to some music?"

    "Yes, thank you":
        jump Yes

    "No thanks":
        jump No

label Yes:

    play sound "audio/guitar.mp3"

label No:

    hide cindy chuckle

    jump Destination_0

label Destination_0:

    show cindy smile at center

    Cindy_ "We used a Dynamic node to ask the previous question. This is a special node that creates a menu and allows you to add multiple outputs, each of which will have its own button."

    Cindy_ "To create or delete an output on the Dynamic node start by clicking on the Button Settings foldout. There you can enter the text to display on the  button and the name of the output port before clicking on the Create New Button. "

    Cindy_ "Alternatively you can click on the Delete Button to view a dropdown list of existing buttons from which to delete."

    Cindy_ "The other node we used was called Sound. Simply add an Audio Source to this node and it will play automatically when it is being read. I add my audio files to a folder called Audio which reflects the behaviour of Ren'Py."

    hide cindy smile

    show cindy reserved at center

    Cindy_ "The last node should always be the Return node. You can think of it as an exit node which tells the program where to finish."

    Cindy_ "There are other nodes that you can explore but they should be a breeze compared to what we have covered already. A full discription of each node can be found in the online documentation."

    Cindy_ "At any time you can right click on the Graph window and select Check For Errors from the menu which will highlight any problem nodes in red. Once fixed choose Check For Errors again to clear the errors."

    Cindy_ "And when you have finished your story you can open the menu again and select Make Ren'Py Script which will create the script file ready to add to Ren'Py. Remember to copy your images and audio files over as well."

    hide cindy reserved

    show cindy direct at center

    Cindy_ "In the Project window, at the bottom left of the Unity window, within the RenpyMaker folder you will find the Graphs folder. This is where I like to add my new graphs."

    Cindy_ "Creating a new graph is as easy as right clicking on this folder and choosing Create and then Renpy Maker from the menu."

    Cindy_ "And lastly, to play your new graph in Unity remember to drag it into the last slot of the Node Parser script attached to the RenPy Maker object which is in the scene. I often forget to do this so it ends up playing the wrong graph. I know you'll remember though."

    hide cindy direct

    show cindy smile at center

    Cindy_ "That's about it for this tutorial. Please check the online documentation for a more detailed look at the various features of Ren'Py Maker."

    hide cindy smile

    show cindy bye at center

    Cindy_ "I hope this tutorial was helpful to you and I look forward to seeing what you create. Bye for now. "

return

