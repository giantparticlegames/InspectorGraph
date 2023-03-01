# Changelog

## [0.9.0-b] TBD

### Added
- Project settings section for Inspector Graph
  - Max Inspector Window Limit
  - Max Preview Mode Limit
  - Filters by type
- Consolidated UI Toolkit UIDocuments in a single catalog file

### Changed
- Reorganized toolbar menu
  - Moved Reset button from toolbar to View menu
  - Added Filters submenu
  - Added Edit menu to open Project Settings
  - Added Help menu with useful links
- Reorganized UXML and USS files under Assets subfolder

### Fixed
- Fixed performance issue when displaying assets like SpriteAtlas that can reference multiple assets ([Issue #2](https://github.com/giantparticlegames/InspectorGraph/issues/2))
- Fixed issue where the connection lines between windows were re-added every time a refresh was called
- Fixed Unity warning about UI Toolkit referenced assets being moved due to absolute path declaration

### Removed
- Removed hard-coded `Show > Scripts` and `Show > Prefab References` menu items (Replaced with Filters)



## [0.8.0-b] 2022-12-17

### Added
- Initial Beta implementation of the tool
