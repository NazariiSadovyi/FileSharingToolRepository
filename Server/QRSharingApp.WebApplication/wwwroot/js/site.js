"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/fileHub").build();

connection.on("FileAdded", function (id, fileName, adress, thumbnailAdress, qRCodeAdress, extension, isVideo) {
    var htmlContent = "";
    if (isVideo) {
        htmlContent = "<div id=\"" + id + "\" class=\"border border-4 photo-thumbnail-item\" onclick=\"ThumbnaiClick(this)\" data-video=\'{\"source\": [{\"src\":\"" + adress + "\", \"type\":\"video/"
            + extension + "\"}], \"attributes\": {\"preload\": true, \"playsinline\": true, \"controls\": true, \"autoplay\": false, \"loop\": true}}\' data-poster=\""
            + thumbnailAdress + "\" data-sub-html=\" \"> <img class=\"thumbnail-img\" src=\""
            + thumbnailAdress + "\" /> <img class=\"thumbnail-qrcode-img\" src=\"" + qRCodeAdress + "\" /> </div>";
    }
    else {
        htmlContent = "<div id=\"" + id + "\" class=\"border border-4 video-thumbnail-item\" onclick=\"ThumbnaiClick(this)\" data-src=\'"
            + adress + "\' data-sub-html=\" \"> <img class=\"thumbnail-img\" src=\""
            + thumbnailAdress + "\" /> <img class=\"thumbnail-qrcode-img\" src=\"" + qRCodeAdress + "\" /> </div>";
    }

    var element = $(htmlContent);
    $("#lightgallery").prepend(element);

    try {
        if (lightGalleryInstance == null || !lightGalleryInstance.lgOpened) {
            return;
        }

        let galleryItems = null;
        if (isVideo) {
            var videoType = "video/" + extension;
            galleryItems = [
                ...lightGalleryInstance.galleryItems,
                ...[
                    {
                        video: { "source": [{ "src": adress, "type": videoType }], "attributes": { "preload": true, "playsinline": true, "controls": true, "autoplay": true } },
                        thumb: thumbnailAdress,
                    },
                ],
            ];
        }
        else {
            galleryItems = [
                ...lightGalleryInstance.galleryItems,
                ...[
                    {
                        src: adress,
                        thumb: thumbnailAdress,
                    },
                ],
            ];
        }

        lightGalleryInstance.updateSlides(galleryItems, lightGalleryInstance.index);
        lightGalleryInstance.slide(galleryItems.length - 1);
    } catch (e) {
        console.error(e);
    }
});

connection.on("FileRemoved", function (id) {
    try {
        removeElement(id);

        if (lightGalleryInstance == null || !lightGalleryInstance.lgOpened) {
            return;
        }

        var itemToRemove = lightGalleryInstance.galleryItems.find(o => o.thumb.includes(id));
        var newGalleryItems = arrayRemove(lightGalleryInstance.galleryItems, itemToRemove);

        lightGalleryInstance.updateSlides(newGalleryItems, lightGalleryInstance.index);
        lightGalleryInstance.slide(lightGalleryInstance.index);
    } catch (e) {
        console.error(e);
    }
});

connection.start().then(function () {
    console.log("SignalR connection initialized")
}).catch(function (err) {
    return console.error(err.toString());
});

function removeElement(id) {
    var elem = document.getElementById(id);
    return elem.parentNode.removeChild(elem);
}

function arrayRemove(arr, value) {
    return arr.filter(function (ele) {
        return ele != value;
    });
}