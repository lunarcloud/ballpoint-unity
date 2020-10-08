LIST inventory = stick, key, dog_collar, rope, (lint), improvised_fishing_pole, leash
-> story

== function pickup(item) ==
~ inventory -= lint
~ inventory += item
~ return

== function combine(items) ==
{
    - items ? (stick, rope):
        ~ inventory -= items
        ~ inventory += improvised_fishing_pole
    - items ? (dog_collar, rope):
        ~ inventory -= items
        ~ inventory += leash
}
~ return


== story
+ [inventory] 
    I've got... {inventory}
+ [combine]
  ** {inventory ? (stick, rope)} [stick and rope]
    ~ combine(stick + rope)
  ** {inventory ? (dog_collar, rope)} [dog_collar and rope]
    ~ combine(dog_collar + rope)
  ++ {CHOICE_COUNT() < 1} [nothing]
+ [pickup] 
  ** [stick] 
    ~ pickup(stick)
  ** [key] 
    ~ pickup(key)
  ** [dog_collar] 
    ~ pickup(dog_collar)
  ** [rope] 
    ~ pickup(rope)
  ++ {CHOICE_COUNT() < 1} [nothing]
* {inventory ? improvised_fishing_pole} [go fish]
    And so I went fishing.
    -> END
* {inventory ? leash} [walk dog]
    The dog was very happy to go for a walk.
    -> END
- -> story