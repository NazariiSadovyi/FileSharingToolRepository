"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/fileHub").build();

connection.on("FileAdded", function (id, fileName, adress, thumbnailAdress, qRCodeAdress, extension, isVideo) {
    console.log(id, fileName, adress, thumbnailAdress, qRCodeAdress, extension, isVideo);
    var htmlContent = "";
    if (isVideo) {
        htmlContent = "<div class=\"border border-4 photo-thumbnail-item\" data-video=\'{\"source\": [{\"src\":\"" + adress + "\", \"type\":\"video/"
            + extension + "\"}], \"attributes\": {\"preload\": true, \"playsinline\": true, \"controls\": true, \"autoplay\": true, \"loop\": true}}\' data-poster=\""
            + thumbnailAdress + "\" data-sub-html=\" \"> <img class=\"thumbnail-img\" src=\""
            + thumbnailAdress + "\" /> <img class=\"thumbnail-qrcode-img\" src=\"" + qRCodeAdress + "\" /> </div>";
    }
    else {
        htmlContent = "<div class=\"border border-4 video-thumbnail-item\" data-src=\'"
            + adress + "\' data-sub-html=\" \"> <img class=\"thumbnail-img\" src=\""
            + thumbnailAdress + "\" /> <img class=\"thumbnail-qrcode-img\" src=\"" + qRCodeAdress + "\" /> </div>";
    }

    try {
        var element = $(htmlContent);
        $("#lightgallery").append(element);
    } catch (e) {
        console.error(e);
    }

    try {
        if (!lightGalleryInstance.lgOpened) {
            setTimeout(() => {
                lightGalleryInstance.destroy();
            }, 100);
            setTimeout(() => {
                lightGalleryInstance = lightGallery(lgDemoUpdateSlides, {
                    plugins: [lgThumbnail, lgVideo],
                    thumbnail: true,
                    videojs: false,
                    autoplayVideoOnSlide: true,
                    autoplayFirstVideo: false
                });
            }, 200);
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
        slidesUpdated = true;
    } catch (e) {
        console.error(e);
    }
});

connection.start().then(function () {
    console.log("SignalR connection initialized")
}).catch(function (err) {
    return console.error(err.toString());
});