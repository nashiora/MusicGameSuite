
# music:theori Roadmap

## Short Term

### NeuroSonic Game Mode

+ Proper gameplay
  - [ ] Playable Buttons
  - [ ] Playable Lasers
    *Yes, those are separate tasks*
+ Chat Selection Screen
  Some of this will be important for other game modes, and I'd like to see a collective chart select with all supported game modes in it, but ideally each game mode can have their own separate chart select screen first. Charts will, in future, have the same format which should make building a chart select from a base implementation of features extremely easy.
  - [ ] Chart databasing
    This is the part which should be ready for a switch to a new format. Currently, the game supports loading `.ksh` files only and converts to the internal representation afterwards. The internal representation needs reworked to be more flexible or at least support a generic dictionary-based base object.
    
    Speaking of:
  - [ ] More flexible internal chart representation
    - [ ] Generic dictionary-based base object
    - [ ] Allow for more specific subtypes if necessary
      This feature might be *too* much for short term, but would be nice for code targeting something with a lot of specific features so type checking can aid that.
+ Miscellaneous
  - [ ] Responsive highway view configuration
    Mostly for development aid, but also to facilitate user configuration without breaking the system. It might be nice to have a few settings to make playing easier for some people. Accessability, man.

## Long Term
