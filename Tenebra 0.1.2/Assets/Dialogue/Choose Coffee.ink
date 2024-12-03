INCLUDE Globals.ink

{coffee_name == "": -> main | -> already_chosen }+
-> main

=== main ===
How are you feeling today?
+ [Happy]
    That makes me feel <color=\#F8FF30>happy</color> as well! #portrait:barista
+ [Sad]
    Oh, well that makes me <color=\#5B81FF>sad</color> too. #portrait:barista

- Well, would you want to drink something? #portrait:tavern_keeper 
+ [Yes]
    -> drinks
+ [No]
    Goodbye then! #order:1 
    -> END
    
=== drinks ===
What would you want to drink?
+ [Espresso]
~ coffee_name = "Espresso"
    Oh, here it is.
+ [Cocoa Milk]
~ coffee_name = "Cocoa Milk"
    There you go!
    
- Would you want something more?
+ [Yes]
    -> drinks
+ [No]
    Goodbye then! #order:1 
    -> END
    
=== already_chosen ===
Strange!
{coffee_name == "Espresso":
    I am feeling more darkness in me.
    Like something inside me makes me desire darkness.
} 
{coffee_name == "Cocoa Milk":
    What a beautiful day.
    I am feeling much happier than before.
}
->END
