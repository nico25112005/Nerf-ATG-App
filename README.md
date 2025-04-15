# Nerf-ATG-App

## Overview

The **Nerf-ATG-App** is an Augmented Reality (AR) game designed specifically for interaction with **Nerf Laser Ops Blasters**. "ATG" stands for **Advanced Tag Game**, a sophisticated lasertag experience that combines the physical action of Nerf blasters with digital gameplay mechanics. The app leverages **Bluetooth** to connect with the blasters and **GPS** to enable location-based missions and battles. Players can compete against each other in immersive environments, upgrade virtual weapons, and enhance their gear through an in-game shop.

Built in **Unity** and optimized for Android devices, the app offers an intuitive user interface that allows players to start missions, adjust settings, and track progress. Whether playing solo or in teams, the Nerf-ATG-App elevates the lasertag experience with digital enhancements, creating a unique and engaging gameplay environment.

## Key Features

- **Bluetooth Connectivity**: Seamlessly pairs with Nerf Laser Ops Blasters for an interactive gaming experience.
- **GPS-Based Mechanics**: Utilizes the player’s location for location-based missions, battles, or events, similar to other AR games.
- **In-Game Shop**: Enables players to purchase virtual upgrades, new weapons, or cosmetic items.
- **Augmented Reality**: Overlays digital content onto the real world to enhance the lasertag experience.
- **User-Friendly Interface**: Intuitive menus for starting games, managing equipment, and tracking progress.
- **Player Progression**: Allows players to improve their skills and gear through missions and purchases.
- **Multiplayer Mode**: Supports team or solo battles with other players nearby (dependent on GPS data).

## Technical Details

The Nerf-ATG-App was developed in **Unity** and relies on a combination of C# scripts and Unity packages to deliver its functionality. Key technical components include:

- **Bluetooth Integration**: Facilitates communication with Nerf Laser Ops Blasters to track real-time actions like shots, hits, or ammo levels.
- **GPS Functionality**: Uses device location data to tie gameplay elements to real-world locations, such as missions or virtual checkpoints.
- **Unity Packages**: Incorporates packages like `TextMeshPro` (version 3.0.6), `Timeline`, `UGUI`, and others for text rendering, animations, and user interfaces.
- **AR Technology**: Likely based on Unity’s built-in AR features or AR Foundation to project digital content into the real world (exact implementation not explicitly documented in the repository).

## Prerequisites

To use or develop the Nerf-ATG-App, you will need:

### For Players
- **Android Device**: A smartphone or tablet running Android with Bluetooth and GPS capabilities.
- **Nerf Laser Ops Blaster**: A compatible blaster, such as the Nerf Laser Ops Pro AlphaPoint or DeltaBurst (Bluetooth-enabled). Without a blaster, some features may be limited.
- **Internet Connection**: Required for GPS data, updates, or potential multiplayer features.
- **Permissions**: Location access (for GPS) and Bluetooth access must be enabled.

### For Developers
- **Unity**: A Unity version compatible with TextMeshPro 3.0.6 or higher (recommended: Unity 2021 LTS or newer).
- **Development Environment**: A PC or Mac with Unity Editor and Android Build Support installed.
- **Android SDK**: For building and deploying to Android devices.
- **Git**: To clone the repository.
- **Basic Knowledge of Unity and C#**: For customization or further development.

## Installation

### For Developers
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/nico25112005/Nerf-ATG-App.git
