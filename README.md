# RoomMapper - A simple room mapping scene for Magic Leap One.

# What is it?
This is supposed to be a simple unity scene you can use to instruct your user to map the environment in your magic leap applications and demos. It should be lightweight and simple to integrate.

# How do I use it?
## Easy Way
1.  Download the latest unitypackage from the Releases tab
2.  Open your Magic Leap unity project (You should have already imported the MagicLeap asset package)
3.  Select Assets > Import Package > Custom Package... and select the unitypackage you just downloaded
4.  Add RoomMapperScene to your "Scenes in Build" list
5.  Open RoomMapperScene and select the 'Target Manager' GameObject. Set the 'Main Scene' variable to the name of the scene you wish to transition to after mapping

## Other Way
1. Clone this repo
2. Copy the entire repo into your Magic Leap unity project. (Make sure you have the MagicLeap package imported first!)
3. Do steps 3-5 from 'Easy Way'.

# Anything else I should know?
- This will likely be in quite a bit of flux as I iterate on it for my own projects. Feedback and pull requests welcome!
- The license is MIT for both the source and assets, feel free to use them as you wish.
- If you find this at all useful, I'd love to hear! I'm @rje on twitter.
