# Nerf-ATG-App

## Overview

The **Nerf-ATG-App** is a mobile application designed to enhance the lasertag experience with **Nerf Laser Ops Blasters** by connecting via **Bluetooth** to a **custom PCB** (Printed Circuit Board) integrated with the blasters. "ATG" stands for **Advanced Tag Game**, a sophisticated lasertag game that combines physical gameplay in outdoor environments with a digital interface. The app visualizes critical game metrics such as health, shots fired, and player location on an Android device, while the game itself is played physically outdoors, mirroring the traditional Nerf Laser Ops Blaster experience.

Developed in **Unity** and optimized for Android devices, the app provides an intuitive digital interface to manage gameplay, track progress, and enhance the outdoor lasertag experience. Players can engage in missions, upgrade virtual equipment, and monitor real-time game data, all while competing in natural settings like parks or forests.

## Key Features

- **Bluetooth Connectivity**: Connects to a custom PCB integrated with Nerf Laser Ops Blasters to relay real-time game data.
- **Digital Interface**: Displays health, shots fired, player location, and other metrics on the mobile device.
- **GPS-Based Mechanics**: Utilizes the player’s location to track positions and enable location-based missions or objectives in outdoor environments.
- **In-Game Shop**: Allows players to purchase virtual upgrades or items to enhance their gameplay experience.
- **User-Friendly Interface**: Intuitive menus for starting games, managing settings, and visualizing game data.
- **Player Progression**: Enables players to improve their virtual gear and stats through missions and in-game purchases.
- **Outdoor Gameplay**: Designed for physical lasertag battles in natural settings, maintaining the core experience of Nerf Laser Ops Blasters.

## Technical Details

The Nerf-ATG-App is built in **Unity**, leveraging C# scripts and Unity packages to deliver its functionality. Key technical components include:

- **Bluetooth Integration**: Communicates with a custom PCB in Nerf Laser Ops Blasters to track actions such as shots, hits, and health status in real time.
- **GPS Functionality**: Uses device location data to monitor player positions and integrate location-based objectives in outdoor environments.
- **Digital Interface**: Renders game metrics (health, shots, location) on the mobile device for a seamless player experience.
- **Unity Packages**: Incorporates packages like `TextMeshPro` (version 3.0.6), `Timeline`, `UGUI`, and others for text rendering, animations, and user interfaces.

## Prerequisites

To use or develop the Nerf-ATG-App, you will need:

### For Players
- **Android Device**: A smartphone or tablet running Android with Bluetooth and GPS capabilities.
- **Nerf Laser Ops Blaster with Custom PCB**: A compatible blaster (e.g., Nerf Laser Ops Pro AlphaPoint or DeltaBurst) equipped with a custom Bluetooth-enabled PCB. Without the custom PCB, core features will be unavailable.
- **Internet Connection**: Required for GPS data or potential multiplayer features.
- **Permissions**: Location access (for GPS) and Bluetooth access must be enabled.

### For Developers
- **Unity**: A Unity version compatible with TextMeshPro 3.0.6 or higher (recommended: Unity 2021 LTS or newer).
- **Development Environment**: A PC or Mac with Unity Editor and Android Build Support installed.
- **Android SDK**: For building and deploying to Android devices.
- **Git**: To clone the repository.
- **Basic Knowledge of Unity and C#**: For customization or further development.
- **Custom PCB Specifications**: Documentation or details about the PCB’s Bluetooth protocol for integration and testing.

## Installation

### For Developers
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/nico25112005/Nerf-ATG-App.git
