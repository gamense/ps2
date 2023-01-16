# Gamense

Buttplug.io app for PS2 using Census

## Disclaimer

This is my first Desktop application and using this library, so there might (likely) be bugs. Please use your toys responsibly. Closing Gamense will immediately stop the vibration if needed. If you are somehow hurt while using this application, I am not responsible. Follow all safety instructions of the toy you are using.

## Installation

1. Install Intiface Central: https://intiface.com/central/
    - This is the bridge between the application and your toys
2. In Intiface Central, click Settings, and turn on the Device Managers you are using
    - For a Hush, this is Bluetooth LE (which is on by default)
    - You don't actually need the Lovense adapter: https://how.do.i.get.buttplug.in/hardware/lovense.html
3. Install Visual Studio 2022 Community Edition (other editions work if you have them installed)
4. Install these packages:
    - .NET desktop development
    - Universal Windows Platform development
5. Click run or press F5 hotkey

## Setup

1. Launch Intiface Central
2. Start the Intiface Central server by pressing the Play button in the top left
3. Connect your toy to Intiface Central
    - It'll be listed in the Devices tab
4. Launch gamense
5. In the Connect to toy tab, press Connect. If it worked, it will list your toy 
6. In the Setup actions tab, enter your character name, then press Add character
7. If it worked, the character will show up in the characters list below

This is my first WPF application and graphic design in not my passion. I know the UI is shit

## Actions

Different actions will increase (or sometimes decrease) how strongly your toy vibrates. Each action grants you points. If the action is enough to push the points over a threshold, it will increase the vibration strength by one. Your points will decay over time, and if low enough, will decrease the vibration strength. Higher vibration strengths decay faster.

### Actions.json
The Actions.json file determines how much the vibration of your toys will change for different actions. If you'd like to change them, you can do so. The Name field is what's displayed in the UI. The Key field is what is actually used. Kill and Death are what they say. The GainExperience actions are based on the experience_id and who performed or received the action. GainExperience.Source is used when one of the tracked characters performed the action (was the character_id field from Census), while GainExperience.Other is used if one of the tracked characters was the target of the action (the other_id field from Census). The experience IDs match Census: https://census.daybreakgames.com/s:example/get/ps2:v2/experience?c:limit=1000

For example if you'd like to add resupplying increases the strength, you'd want to add GainExperience.Source.34
