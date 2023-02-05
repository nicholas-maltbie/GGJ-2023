# GGJ Roots Project 2023

This is a multiplayer game where each player attempts to take control of the
forest through growing their own environment through a network of roots
and plants they grow across the map.

When placing plants, each player will gain some score.
First player to 100 points wins the game.

Some plants produce resources, some extend your grow range
and some just give victory points.

## How to Play

The game is inherently multiplayer but there is an option to `Play Offline`
if you want to play on your own to test out the game mechanics.

When you start a game, all players will be thrown into a lobby.

The host can start the game whenever they are ready by hitting
the `Start Game` button.

Once you start the game each player will spawn in with a tree and two
producer plants.

You can move your player around with `WASD` and place plants with the
`space` key. Use the mouse wheel to scroll between different plants
you want to play or click the plant on the bottom left panel
in the Plant Tableau.

![Labeled player tableau](../resources/Player-UI.png)

## Placement Rules

There are some rules about how you can place plants, you can't just put them
anywhere on the map.

Plants need to be put within the `Grow Range` of your other plants.
The grow zone is the big green space that is visible around plants
with a grow zone like a cactus or tree.

Plants of the same type cannot be placed too close to each other as well.
That is shown via the restricted range which is a red space around the
plant that the player can't place stuff. This will change based
on which plant you have selected.

Each plant must be connected to another plant by a root within your network.
When you are attempting to place the plant, it will show the possible
roots on the map. If your placement is valid, the plant will be green
and highlighted on the screen.

![Valid Placement](../resources/Valid-Green.png)

If the plant overlaps with an existing plant or if no roots can be drawn
to one of your grow zones without overlapping, it will be red to show
you cannot play.

![All overlapping](../resources/All-Overlap.png)

If one of the roots is overlapping but others are not, all valid roots
will be drawn.

![Some Overlap](../resources/One-Overlap.png)

If the plant is too far away and can't draw any roots, it will be a grey
color.

![Too Far](../resources/Too-Far-Away-Grey.png)

And if the placement is valid but you don't have the resources to play it,
the icon will be yellow and you can look at the display dialog in the
bottom right to see which resources you are missing.

![Too Poor](../resources/Too-poor.png)
