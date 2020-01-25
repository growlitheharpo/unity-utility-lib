# "KeatsLib" - A Unity utility collection

"KeatsLib" (name somewhat tongue-in-cheek) started off as a collection of classes and methods that I found myself constantly rewriting when making Unity projects in college. It grew over time, with a particular leap in growth after my internship at a professional company that used Unity.

The "classic-master" branch represents the state of this library at the end of my junior year of college. The "master" branch includes this, plus a large chunk of code that was written for my senior capstone project [re[Mod]](https://jameskeats.com/portfolio/remod.html) which, a couple of years later, I backported into a more generic form. I didn't prune those references particularly thoroughly, so there may still be things lingering here that would not make sense for a totally generic library.

For that capstone project, we used FMOD Studio 1.10 as our audio engine, and AudioManager.cs is conspicuously missing from this library because the implementation was 100% tied to FMOD.

I don't expect that anyone will come along and actually use this library for anything, but if you chose to, it is released under the MIT license (see license.txt). I'd love to hear from you if you chose to use this, but contacting me/requesting permission is not required.
