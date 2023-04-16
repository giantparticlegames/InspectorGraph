# Giant Particle - Inspector Graph <!-- omit in toc -->
The Inspector Graph is a tool to better visualize, understand and manipulate objects in a reference hierarchy.

![Window Sample](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/WindowSample.png)

## Index <!-- omit in toc -->
- [Main Features](#main-features)
- [Change log](#change-log)
- [Project Settings](#project-settings)
  - [Visualization settings](#visualization-settings)
  - [Filters](#filters)
- [Main Window](#main-window)
  - [View Menu](#view-menu)
    - [Filters](#filters-1)
  - [Edit Menu](#edit-menu)
  - [Help Menu](#help-menu)
  - [Inspected Object Reference](#inspected-object-reference)
  - [Zoom Controls](#zoom-controls)
- [Inspector Window](#inspector-window)
  - [Window controls](#window-controls)
  - [Toolbar](#toolbar)
  - [Footer](#footer)
    - [Object Type](#object-type)
    - [Stats](#stats)
    - [Reference Highlight](#reference-highlight)
    - [Window Size Control](#window-size-control)
  - [Views](#views)
    - [Inspector Element](#inspector-element)
    - [IMGUI](#imgui)
    - [Preview](#preview)
    - [Static Preview](#static-preview)
  - [References](#references)
    - [Highlighting](#highlighting)

## Main Features
* Floating inspector window per object
* Multiple View modes available per object
  * Direct Reference
  * Nested Prefab
* Visual representation of references from and to objects
* In-Place field editing
* Zoom control for better visualization
* Per project settings with customization options
* Object type filtering
* and more...

## Change log
Take a look at the latest changes [here](CHANGELOG.md).

## Project Settings
The Project settings section is where the per project settings of this tool are controlled. You can access this section either via the [`Edit`](#edit-menu) menu under the Inspector Graph Window or via Unity `Edit > Project Settings...`.

By default, the data is saved in your project under `Assets/Editor/com.giantparticle.inspector_graph/InspectorGraphSettings.asset`. Feel free to move it to a different place if you want.

![Project Settings](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ProjectSettings.png)

### Visualization settings
* **Max Windows**: Indicate the maximum number of Inspector Windows that will be displayed. `100` is the maximum limit, otherwise Unity starts to complain about it.
* **Max Preview Windows**: Indicate the maximum number of windows that can display the Preview Mode. After this number is reached, new windows will be forced to display a static preview (Icon based on the object type). This is to avoid potential performance issues, feel free to change this value if your computer is capable and/or your project is not that resource intensive.
* **Diagram Connection Colors**: Indicate the colors used for each representation of a reference.
* **Default Window Size**: Indicate the default size of each Inspector Window based on the View Mode

### Filters
Filters are an easy way to control the visibility or expansion of objects of certain types. In this section you can add or remove filters based on your preferences and the types available in your project.

* **Type Full Name**: This is the [fully qualified name](https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/specifying-fully-qualified-type-names) of the object type to filter without the assembly name (See [Type.FullName](https://learn.microsoft.com/en-us/dotnet/api/system.type.fullname?view=net-7.0))
* **Display Type**: Indicate to display, or not, objects of the given type
* **Expand Type**: Indicate to expand the graph from a visible object of the given type

When clicking the **Type Full Name** field, you will see a popup with all available types in your project, just search for the one you are looking for via the search bar and double-click it to autocomplete the field or manually type in the type.

![Type Selection Popup](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/TypeSelectionPopup.png)

## Main Window
The main window has multiple controls for you to use which are explained in this section.

### View Menu
The **View menu** lets you control some visual elements of the graph hierarchy.

![View Menu](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ViewMenu.png)

* **Refresh**: The refresh button lets you update the current graph with any changes that were not picked up while editing. It also rearranges the untouched windows in the graph to accommodate new windows or window positions.
* **Reset**: The reset button allows you to reset all windows to their default state, position and size.

#### Filters
The filters submenu allows you to temporarily control the visibility and/or expansion of inspectors based on the type of object being inspected. By default there are `3` types available: `UnityEngine.GameObject`, `UnityEditor.MonoScript` and `UnityEngine.U2D.SpriteAtlas` you can add or remove these under the [Project Settings section](#project-settings).

![Filters](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ViewMenu-Filters.png)

* **Show**: This option shows or hides the visualization of any reference of the indicated type involved in the hierarchy.

| Show `UnityEditor.MonoScript`</br>Disabled                  | Show `UnityEditor.MonoScript`</br>Enabled                 |
| ----------------------------------------------------------- | --------------------------------------------------------- |
| ![Script View Disabled](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ViewMenu-ShowScripts-Off.png) | ![Script View Enabled](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ViewMenu-ShowScripts-On.png) |

* **Expand**: This option enables or disables the visualization of references within the objects with the indicated type.

> Note: In the case of Prefabs (`UnityEngine.GameObject`), the expansion of them includes references from scripts within the prefab and nested prefabs. Note that all modifications to the nested prefabs are visualized correctly. For example, if a nested prefab reference to a Mesh `A` is modified to be Mesh `B`, Mesh `B` will show up in the graph while Mesh `A` will not.

| Expand `UnityEngine.GameObject`</br>Disabled                                    | Expand `UnityEngine.GameObject`</br>Enabled                                   |
| ------------------------------------------------------------------------------- | ----------------------------------------------------------------------------- |
| ![Prefab References View Disabled](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ViewMenu-ShowPrefabReferences-Off.png) | ![Prefab References View Enabled](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ViewMenu-ShowPrefabReferences-On.png) |

### Edit Menu
The **Edit** menu currently has only one option that opens the Inspector Graph [project settings](#project-settings).

![Edit Menu](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/EditMenu.png)

### Help Menu
The **Help Menu** has useful links

![Help Menu](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/HelpMenu.png)

* **Documentation**: Links to this document
* **Report a bug**: Link to the GitHub page to create a new issue
* **Website**: Link to the [Official website](http://www.giantparticlegames.com) of this tool

### Inspected Object Reference
This field is located at the upper right corner and allows you to assign a reference of an object you want to inspect.

![Inspected Object Reference](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectedObjectReference.png)

### Zoom Controls
The Zoom controls are located at the bottom left corner of the main window and give you control of the scale of the visualization. The zoom level is controlled by the slider and the reset button will reset the zoom to 1.
![Zoom Controls](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/ZoomControls.png)

## Inspector Window
The inspector window is a small version of the Unity inspector with extra functionality to better visualize references.

![Inspector Window](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow.png)

### Window controls
Currently, the window can be minimized or extended using the buttons at the top left corner.

![Window Controls](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-WindowControls.png)

* Minimize: The minimize button (Yellow) will collapse the window leaving only the header and the window controls visible.

![Window - Minimized](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-Minimized.png)

* Extend: The extend button (Green) will eliminate the height restrictions allowing for the entire inspector to be displayed.

![Window - Extended](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-Extended.png)

### Toolbar
The toolbar provides a reference to the object being presented in the window and options to switch the representation of it (See the [Views](#views) section for more information).

![Window - Toolbar](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-Toolbar.png)

### Footer
At the bottom of the window you will be able to see more information and controls.

![Window - Footer](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-Footer.png)

#### Object Type
The object type name is shown at the left corner. When hovering, a tooltip will be displayed with the full name of the type.

#### Stats
The following information is displayed next to the object name:
* Number of references to the object in the graph
* Number of references from the object in the graph

> Note: If there are more than one references from/to a single object to/from the object represented in the window, the total number of references will be displayed in brackets (Example: `[Total: 5]`).

#### Reference Highlight
To toggle highlight the references to and from the object represented in the window, simply press the button with a diverging arrow [ <img src="Editor/Assets/Images/Icon-SplitArrows.png" height="14"></img> ] at the right.

#### Window Size Control
To resize an inspector window, press and drag the dotted icon [ <img src="Editor/Assets/Images/Icon-Corner.png" height="14"></img> ] at right.

### Views
The window provides different options to visualize the content. Depending on the object compatibility some options will be disabled.

![View Modes](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-ViewModes.png)

#### Inspector Element
This view mode is available when an object has an inspector that supports [UI Toolkit](https://docs.unity3d.com/Manual/UIElements.html).
> Note: On some occasions UI Toolkit is still supported but it is not displayed correctly.

![Inspector Element Mode](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-ViewMode-InspectorElement.png)

#### IMGUI
This view mode uses the [Immediate Mode GUI](https://docs.unity3d.com/Manual/GUIScriptingGuide.html) inspector to display the content.

![IMGUI Mode](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-ViewMode-IMGUI.png)

#### Preview
This view mode visualizes a preview of the object (If supported). Very Often, Prefabs with visual elements, 3D Models, Textures and other assets will show this option.

![Preview Mode](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-ViewMode-Preview.png)

#### Static Preview
The static preview mode shows an image of the object either taken from the Preview or from the default icon based on the type of object.

| Static Preview - Mesh                                                  | Static Preview - Shader                                                    |
| ---------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| ![Mesh Static Preview](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-ViewMode-Static-Mesh.png) | ![Shader Static Preview](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/InspectorWindow-ViewMode-Static-Shader.png) |

### References
References are represented in the graph as curved arrows. The source of the arrow indicates the object that holds the reference and the end of the arrow represents the reference target.

![Reference](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/Reference.png)

The color of the arrow indicate what type of reference it is:
* White for direct reference on a serialized field
* Cyan for nested prefab references

#### Highlighting
The references to, and from, a specific object will be highlighted by hovering over the header of the inspector window or by activating the highlight lock (See [Footer](#reference-highlight))

| Reference - Normal                               | Reference - Highlighted                                    |
| ------------------------------------------------ | ---------------------------------------------------------- |
| ![Normal References](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/Reference-Normal.png) | ![Highlighted References](https://github.com/giantparticlegames/InspectorGraph/raw/submit_0.10.1b/.docs/Reference-Highlighted.png) |

