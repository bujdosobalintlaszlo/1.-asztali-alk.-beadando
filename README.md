# Western project
# How it works?
1.Step at start we need to give the program how many times we want to run the simulation.
<img width="1115" height="628" alt="image" src="https://github.com/user-attachments/assets/6efdf852-3700-40dc-97a2-f16dab8e0ac3" />
2.step press ENTER and the simulation starts
<img width="1105" height="624" alt="image" src="https://github.com/user-attachments/assets/863a559b-57f1-4eb6-ae19-09dce4346746" />
The simulation will be drawn on the screen. You will be able to see some trait of the sheriff. The healt,gold amount,taken steps and the time passed.
# Goal of the project:
The task states the we need to have a 25x25 sized map.
There are 6 field types:
Ground - Green, it is a regular field with no special trait.
Gold - Yellow, the sheriff needs to collect all of them on the map. If he manages to collect them he needs to find the townhall to return the stolen gold. Bandits also can pick it up.
Whiskey - Dark yellow, heals the sheriffs health by 50. Overheal is not possible, so if the sheriff picks it up at a health level where it would raise his hp over 100, it caps and wont excede 100.
Rock - Grey, barrier nor the sherrif nor the badits are able to step on those field.
Sheriff - Red,the most complex element of the simulation.In the begining he sees in a 3x3 and as he moves he explores the map in 3x3 sections. His number one goal is to collect the golds scattered all around of the map.(we know that there are 5 of them.) After he managed to do that he will go to the townhall. Along his way he will meet obsticles. He can engage in fights with bandits. He will target them mainly if he explored the whole map and kill them in hope of collecting the gold from them. If the sheriff is under 50hp he will try to escape and search for whiskey to heal.
Bandit - Purple, they move randomly, they move faster than the sheriff. They can do 3 actions, move, collect gold if they step on a gold field and also engage in fight.
Townhall - Cian, the place where the sheriff goes to return the gold.

#Explonation
There is a given map which has a fix size 25x25. There are two types of moving objects on the map. One of those is the main character the sheriff. The sheriff
