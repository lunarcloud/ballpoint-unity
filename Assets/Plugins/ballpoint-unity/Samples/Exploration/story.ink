VAR lastKnownSpeaker = "None"
LIST music = SpringChicken, (BeatThee)
LIST scene = Desert

-> Explore

== Explore ==
exploration... #hidedialog
{cactus and robot and B: -> AllExplored }
+ [cactus] -> cactus -> Explore
+ [robot] -> robot -> Explore
+ [B] -> B -> Explore

== cactus ==
#showdialog
What an odd cactus... #said:A
->->

== robot ==
#showdialog
Beep Boop! #said:robot
->->

== B ==
#showdialog
Hey, how's it going? #said:B
->->

== AllExplored ==
And so you interacted with everything, the end.
-> END