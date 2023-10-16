# Made with Ren'Py Maker
# By Alan Baylis
# https://www.renpymaker.com

# 3 Character Nodes

init python:
    class Characters:
        def __init__(self, name: str, color: str):
            self.name = name
            self.color = color

# Character variables
    CharacterList = []
    Cindy = Characters("Cindy", "RGBA(1.000, 0.000, 0.000, 1.000)")
    CharacterList.append(Cindy)
    Micheal = Characters("Micheal", "RGBA(0.000, 0.040, 1.000, 1.000)")
    CharacterList.append(Micheal)
    Lucy = Characters("Lucy", "RGBA(1.000, 0.951, 0.000, 1.000)")
    CharacterList.append(Lucy)

define Cindy_ = Character("Cindy", image="cindy", color="#ff0000")
define Micheal_ = Character("Micheal", image="michael", color="#000aff")
define Lucy_ = Character("Lucy", image="lucy", color="#fff200")

image lucy = "lucy.png"
image michael = "michael.png"
image cindy bye = "cindy_bye.png"
image cindy happy = "cindy_happy.png"

image side cindy = "side_cindy.png"
image side michael = "side_michael.png"
image side lucy = "side_lucy.png"

label Biology:

    scene schoolroom

    show lucy at center

    Lucy_ "Hello, I'm Lucy"

return

label Economics:

    scene schoolroom

    show michael at center

    Micheal_ "Hi, my name is Michael"

return

label Music:

    scene livingroom

    show cindy bye at center

    play music "audio/guitar.mp3" fadeout 0 fadein 0

    Cindy_ "Just listening to music"

return

label start:

    scene bedroom

    show cindy happy at center

    Cindy_ "Good morning!"

    Cindy_ "It's time for school"

    Cindy_ "Which class would you like to attend?"

label Question:

    jump destination_0

label destination_0:

menu:

    Cindy_ "Please choose a class"

    "Economics":
        jump economics_26

    "Biology":
        jump biology_26

    "Music":
        jump music_26

label economics_26:

    call Economics

label destination_1:

label destination_2:

    jump Question

label biology_26:

    call Biology

    jump destination_1

label music_26:

    call Music

    stop music fadeout 1

    jump destination_2

