VAR lastKnownSpeaker = "None"
LIST music = A, (B)
LIST scene = Desert ,(Fall), Forest

This is an example of narration.
* explore a bit[]. #hidedialog
  ~scene = Desert
  * * [done]Ok, now then... #showdialog
 * yes[... and?]?
-

~scene = Forest
And this is from the perspective of me. #said:A
It continues-

~music = A

Until I interrupt. #said:B
With choices even,
 * No[?].
  Well, okay then. #said:A
 * YAASSSS[!?]!
  And from that point onward A decided to... #said:None
  ** Never speak to B again
  ** Hang out with B from dusk til' dawn[] 
		 -> hang_out
  ** YAAAASSSSS[] #said:A
    *** lol
---
-> END

== hang_out ==
~scene = Fall
And so they totally did.

-> END
