# Westco XA Extensions
**Westco XA Extensions** is a set of extensions for **Sitecore Experience Accelerator** module.

Solution uses [Helix](http://helix.sitecore.net/)

Current **Westco XA Extensions** project is compatible with:

| Product   |      Version      |  Revision |
|----------|:-------------:|:------:|
| Sitecore |  **8.2** | rev. 170407 |
| SXA  |  **1.3** | rev. 170412 |

[![License](https://img.shields.io/badge/license-MIT%20License-brightgreen.svg)](https://opensource.org/licenses/MIT)

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

The final Html output may look like the following:

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js" integrity="sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4=" crossorigin="anonymous"></script>
<script>window.jQuery || document.write('<script src="/-/media/Assets/jquery/jquery-3-2-1/Scripts/optimized-min.js?t=20170614T032244Z">\x3C/script>')</script>
<script>console.log('Run after CDN link and fallback test.');</script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.18.1/moment.min.js" integrity="sha256-1hjUhpc44NwiNg8OwMu2QzJXhD8kcj+sJA3aCQZoUjg=" crossorigin="anonymous"></script>
```

---

| [![Michael West](https://gravatar.com/avatar/a2914bafbdf4e967701eb4732bde01c5?s=220)](https://github.com/michaellwest) |
| --- |
| [Michael West](https://michaellwest.blogspot.com) |
