XA.component.staticmap = (function ($, document) {

    "use strict";

    var api = {},
        maps = [],
        searchEndpoint,
        searchResultsSignatures = [],
        key,
        scriptsLoaded = false,
        initialize,
        initialized = false,
        searchResults,
        resultsLoaded,
        myLocationChanged,
        parseHashParameters,
        myLocationData,
        mapViews = [],
        MapModel,
        MapView;
        
    initialize = function () {
        var i, mapsCount = maps.length;
        for (i = 0; i < mapsCount; i++) {
            //crate Backbone.js views - one view per component on the page
            mapViews.push(new MapView({el: maps[i], model: new MapModel()}));
        }
    };

    MapModel = Backbone.Model.extend({
        defaults: {
            dataProperties: {},
            showed: false,
            id: null
        },
        initialize: function() {
        }
    });

    MapView = Backbone.View.extend({
        initialize : function () {
            var dataProperties = this.$el.data(),
                properties = dataProperties.properties;

            if (this.model) {
                this.model.set({dataProperties: properties});
                this.model.set({id: this.$el.find(".map-canvas").prop("id")});
            }

            this.render();
        },
        render : function () {
            var that = this,
                properties = this.model.get("dataProperties");
                const maptype = properties.mode;
                const width = parseInt(properties.width);
                const height = parseInt(properties.height);
                const size = `${width}x${height}`;
                const zoom = this.parseZoom(properties.zoom, 15);
                const key = properties.key;
                let imgUrl = `https://maps.googleapis.com/maps/api/staticmap?maptype=${maptype}&size=${size}&zoom=${zoom}&`;

                let markers = [];
                const pois = properties.Pois;
                const poiCount = pois.length;
                for(let i = 0; i < poiCount; i++) {
                    let poi = pois[i];
                    if (poi.Latitude === "" || poi.Longitude === "") {
                        continue;
                    }

                    markers.push(`markers=icon:${poi.PoiIcon}%7C${poi.Latitude},${poi.Longitude}`)
                }

                imgUrl += markers.join('&');
                imgUrl += `&key=${key}`;
                const $img = $('<img>');
                $img.attr('src', imgUrl);
                $(that.$el).find('.static-map').append($img);
        },
        parseZoom: function (str, defaultValue) {
            var retValue = defaultValue;
            if(str !== null) {
                if(str.length > 0) {
                    if (!isNaN(str)) {
                        retValue = parseInt(str);
                    }
                }
            }
            return retValue;
        }
    });

    api.init = function() {
        var i,
            mapElements = $(".static-map.component:not(.initialized)"),
            count = mapElements.length;

        if (typeof (XA.component.search) !== "undefined") {
            //if the page was reloaded there could be situation that search results will load results faster then map initialization
            //so that we have to save them and pass to the models when they will be created
            XA.component.search.vent.on("results-loaded", resultsLoaded);
        }

        if (count > 0) {
            for (i = 0; i < count; i++) {
                var $map = $(mapElements[i]);
                var properties = $map.data("properties");
                key = properties.key;
                $map.addClass("initialized");
                maps.push($map);
            }

            initialize();
        }
    };

    return api;

}(jQuery, document));

XA.register("static-map", XA.component.staticmap);
