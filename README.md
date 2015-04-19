# ILPatcher
Creating patches for C# and VB binarys on IL level

### The Idea
Modifying DotNet \*.dll and \*.exe IL-Instructions with most Editors is unhandy and has to be done everytime the target file gets an update.  
Aim of the ILPatcher is to create patches for IL-Instructions, so the same patch routines can be executed each time on an updated file.  
Furthermore I wanted to create a fast, easy to use, powerful interface for easier work with IL Instructions.

### Features
- (Working) Patchtypes
 - *ILMethodFixed*: Patches one specific Method
 - *MethodCreator*: Creates a new Method and uses another PatchAction to fill it
- Using more PatchActions as Plug-In (atm only works by adding a PatchAction to the source)

### Status
|master|testbranch|
|:--:|:--:|
|[![Build Status](https://travis-ci.org/Splamy/ILPatcher.svg?branch=master)](https://travis-ci.org/Splamy/ILPatcher)|[![Build Status](https://travis-ci.org/Splamy/ILPatcher.svg?branch=testbranch)](https://travis-ci.org/Splamy/ILPatcher)|

### The fantastic libraries I use
- [Mono.Cecil](https://github.com/jbevain/cecil)
- [FastColoredTextBox](https://github.com/PavelTorgashov/FastColoredTextBox)
- [Reflexil](https://github.com/sailro/reflexil) (some snippets)
- [ILSpy/ICSharpCode.Decompiler](https://github.com/icsharpcode/ILSpy)
