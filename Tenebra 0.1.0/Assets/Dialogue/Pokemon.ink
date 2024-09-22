INCLUDE Globals.ink

{pokemon_name == "": -> main | -> already_chosen }+

-> main

=== main ===
Which pokemon do you choose?
    + [Charmander]
        -> chosen("Charmander")
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
~ pokemon_name = pokemon
You chose {pokemon}!
Letsgoo
-> END

=== already_chosen ===
You already chose {pokemon_name}!
-> END