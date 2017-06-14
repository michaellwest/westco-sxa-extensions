# Westco XA Extensions
**Westco XA Extensions** is a set of extensions for **Sitecore Experience Accelerator** module.

Solution uses [Helix](http://helix.sitecore.net/)

Current **Westco XA Extensions** project is compatible with:

| Product   |      Version      |  Revision |
|----------|:-------------:|:------:|
| Sitecore |  **8.2** | rev. 170407 |
| SXA  |  **1.3** | rev. 170412 |

## Setup

Getting started is fairly straightforward.

Create a new tenant and ensure that the _Asset Include_ feature is enabled.

![Imgur](http://i.imgur.com/HS975qI.png)

Create a new site and ensure that the _Geospatial_ and _Maps_ features are enabled. These two are similar to those OOTB with SXA but offer a Static Maps component.

![Imgur](http://i.imgur.com/eRwHQDd.png)

## Assets served by a CDN with local fallback

In the _Media Library_ you simply create new **Assets** to represent your JavaScript and CSS resources from a CDN.

![Imgur](http://i.imgur.com/RyjOryS.png)

You can associate assets at a site level by specifying on the **Settings** item in the **Asset Configuration** section.

![Imgur](http://i.imgur.com/bHCxUcC.png)

You can associate assets on a page level by specifying in the **Asset Configuration** section.

![Imgur](http://i.imgur.com/JLtUWlB.png)

Finally, see that the Html is injected in the `<head>` and `<body>`.

![Imgur](http://i.imgur.com/xNO4dy2.png)


