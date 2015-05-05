(function () {
    //https://github.com/gtramontina/draggable.js/blob/master/draggable.js
    var draggable = (function definition() {
        function addEventListener(element, eventName, handler) {
            if (element.addEventListener) {
                element.addEventListener(eventName, handler, false);
            } else if (element.attachEvent) {
                element.attachEvent('on' + eventName, handler);
            } else {
                element['on' + eventName] = handler;
            }
        }
        function removeEventListener(element, eventName, handler) {
            if (element.removeEventListener) {
                element.removeEventListener(eventName, handler, false);
            } else if (element.detachEvent) {
                element.detachEvent('on' + eventName, handler);
            } else {
                element['on' + eventName] = null;
            }
        }

        var currentElement;
        var currentDropSelector;
        var currentDropElement;
        var currentOptions;
        var fairlyHighZIndex = '10';

        function draggable(options) {
            var element = options.element;
            var handle = options.handle || element;
            var position = options.position;
            var dropSelector = options.dropSelector;
            var dragElement = setPositionType(element, position, options.dragHelperClass);
            var on = 0;
            var started = false;

            setDraggableListeners(dragElement);

            addEventListener(document, "mousemove", function (event) {
                if (on != 2 || started) {
                    return;
                }

                if (position == "static") {
                    copyElement(element, dragElement);
                }

                currentOptions = options;
                startDragging(event, dragElement);
                started = true;
            });

            addEventListener(handle, "mousemove", function (event) {
                event.preventDefault();
            });

            addEventListener(handle, "mousedown", function (event) {
                on = 1;

                setTimeout(function () {
                    if (on == 1) {
                        on = 2;
                    }
                }, 50);
            });

            addEventListener(document, "mouseup", function (event) {
                on = 0;
                started = false;
            });

            return dragElement;
        }

        function setPositionType(element, position, dragHelperClass) {
            if (position == "static") {
                var div = document.createElement("div");
                
                div.className = dragHelperClass;
                div.style.position = "fixed";
                element.parentNode.insertBefore(div, element);

                return div;
            }

            if (!position) {
                position = "absolute";
            }

            element.style.position = position;

            return element;
        }

        function setDraggableListeners(element) {
            element.draggableListeners = {
                start: [],
                drag: [],
                stop: []
            };
            element.whenDragStarts = addListener(element, 'start');
            element.whenDragging = addListener(element, 'drag');
            element.whenDragStops = addListener(element, 'stop');
        }

        function copyElement(from, to) {
            var rec = from.getBoundingClientRect();
            to.innerText = from.innerText;
            to.style.width = inPixels(from.offsetWidth);
            to.style.height = inPixels(from.offsetHeight);
            to.style.left = inPixels(rec.left);
            to.style.top = inPixels(rec.top);
        }

        function startDragging(event, element) {
            clearSelection();

            currentElement && sendToBack(currentElement);
            currentElement = bringToFront(element);
            currentElement.classList.add(currentOptions.draggingClass);

            var initialPosition = getInitialPosition(currentElement);
            currentElement.style.left = inPixels(initialPosition.left);
            currentElement.style.top = inPixels(initialPosition.top);
            currentElement.lastXPosition = event.clientX;
            currentElement.lastYPosition = event.clientY;

            var okToGoOn = triggerEvent('start', { x: initialPosition.left, y: initialPosition.top, mouseEvent: event });
            if (!okToGoOn) return;

            addDocumentListeners();
        }

        function stopDragging(event) {
            removeDocumentListeners();

            var left = parseInt(currentElement.style.left, 10);
            var top = parseInt(currentElement.style.top, 10);

            if (currentDropElement) {
                currentDropElement.classList.remove(currentOptions.dragHoverClass);
            }

            triggerEvent('stop', { x: left, y: top, mouseEvent: event, dropElement: currentDropElement, dragElement: currentElement });
            currentElement.classList.remove(currentOptions.draggingClass);
        }

        function addListener(element, type) {
            return function (listener) {
                element.draggableListeners[type].push(listener);
            };
        }

        function triggerEvent(type, args) {
            var result = true;
            var listeners = currentElement.draggableListeners[type];
            for (var i = listeners.length - 1; i >= 0; i--) {
                if (listeners[i](args) === false) result = false;
            };
            return result;
        }

        function sendToBack(element) {
            var decreasedZIndex = fairlyHighZIndex - 1;
            element.style['z-index'] = decreasedZIndex;
            element.style['zIndex'] = decreasedZIndex;
        }

        function bringToFront(element) {
            element.style['z-index'] = fairlyHighZIndex;
            element.style['zIndex'] = fairlyHighZIndex;
            return element;
        }

        function addDocumentListeners() {
            addEventListener(document, 'selectstart', cancelDocumentSelection);
            addEventListener(document, 'mousemove', repositionElement);
            addEventListener(document, 'mouseup', stopDragging);
        }

        function getInitialPosition(element) {
            var rec = element.getBoundingClientRect();
            var style = element.currentStyle || window.getComputedStyle(element);

            return {
                top: rec.top - parseInt(style.marginTop),
                left: rec.left - parseInt(style.marginLeft)
            };
        }

        function inPixels(value) {
            return value + 'px';
        }

        function cancelDocumentSelection(event) {
            event.preventDefault && event.preventDefault();
            event.stopPropagation && event.stopPropagation();
            event.returnValue = false;
            return false;
        }

        function repositionElement(event) {
            event.preventDefault && event.preventDefault();
            event.returnValue = false;

            var style = currentElement.style;
            var elementXPosition = parseInt(style.left, 10);
            var elementYPosition = parseInt(style.top, 10);

            var elementNewXPosition = elementXPosition + (event.clientX - currentElement.lastXPosition);
            var elementNewYPosition = elementYPosition + (event.clientY - currentElement.lastYPosition);

            style.left = inPixels(elementNewXPosition);
            style.top = inPixels(elementNewYPosition);

            currentElement.lastXPosition = event.clientX;
            currentElement.lastYPosition = event.clientY;

            highlightDropElement(event);

            triggerEvent('drag', { x: elementNewXPosition, y: elementNewYPosition, mouseEvent: event });
        }

        function highlightDropElement(event) {
            currentDropElement = null;

            if (!currentOptions.dropSelector) {
                return null;
            }

            var items = document.querySelectorAll(currentOptions.dropSelector);

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var rec = item.getBoundingClientRect();
                var x = event.clientX;
                var y = event.clientY;

                if (x >= rec.left && x <= rec.right && y >= rec.top && y <= rec.bottom) {
                    currentDropElement = item;
                    item.classList.add(currentOptions.dragHoverClass);
                } else {
                    item.classList.remove(currentOptions.dragHoverClass);
                }
            }
        }

        function removeDocumentListeners() {
            removeEventListener(document, 'selectstart', cancelDocumentSelection);
            removeEventListener(document, 'mousemove', repositionElement);
            removeEventListener(document, 'mouseup', stopDragging);
        }

        function clearSelection() {
            if (window.getSelection) window.getSelection().removeAllRanges();
            else if (document.selection) document.selection.empty();
        }

        return draggable;
    })();

    Polymer("juicy-draggable", {
        domReady: function () {
            var element = null;
            var handle = null;
            var that = this;

            if (!this.elementId) {
                element = this.children[0];
            } else {
                element = this.querySelector("#" + this.elementId);
            }

            if (this.handleId) {
                handle = this.querySelector("#" + this.handleId);
            }

            this.dragHelperClass = this.dragHelperClass || "drag-helper";
            this.dragHoverClass = this.dragHoverClass || "drag-hover";
            this.draggingClass = this.draggingClass || "dragging";

            element = draggable({
                element: element, 
                handle: handle, 
                position: this.position, 
                dropSelector: this.dropSelector,
                dragHelperClass: this.dragHelperClass,
                dragHoverClass: this.dragHoverClass,
                draggingClass: this.draggingClass
            });

            element.whenDragStarts(function (e) {
                that.fire("juicy-draggable-start", e);
            });

            element.whenDragging(function (e) {
                that.fire("juicy-draggable-move", e);
            });

            element.whenDragStops(function (e) {
                that.fire("juicy-draggable-stop", e);
            });
        },
        getScreenSize: function () {
            var w = window;
            var d = document;
            var e = d.documentElement;
            var g = d.getElementsByTagName('body')[0];
            var x = w.innerWidth || e.clientWidth || g.clientWidth;
            var y = w.innerHeight || e.clientHeight || g.clientHeight;

            return { x: x, y: y };
        }
    });
})();