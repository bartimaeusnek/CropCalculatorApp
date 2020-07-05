let toggle = false;

function toCropList(e) {
    if ((e.ctrlKey && toggle)){
        e.preventDefault();
        window.location.href = e.target.href;
        return;
    }
    if ((toggle && !e.ctrlKey) || (e.ctrlKey && !toggle)) {
        e.preventDefault();
        $(e.target).href=""
        const text = $(e.target)
            .text()
            .replace(/\n {20}Parent \d:/,"")
            .replace(/\n\s/,"")
            .replace(/\n\s/,"")
            .trim();
        window.location.href = window.location.href.replace(/crop=(?:.*[^&])/,`crop=${text}`);
    }
}

function toggleMode() {
    toggle = !toggle;
    const element = document.getElementById("toggle");
    const tooltips = document.getElementsByClassName("wrapper-box-left color-crops-wrapper color-crops-wrapper-border")
    if (toggle) {
        element.textContent = "Go to crop mode. Click/Tap here to toggle!"
        for (const div of tooltips){
            div.setAttribute("data-original-title","Go to crop")
        }
    }
    else {
        element.textContent = "Ignore this crop mode. Click/Tap here to toggle!"
        for (const div of tooltips){
            div.setAttribute("data-original-title","Ignore this crop")
        }
    }
}

$(document).keydown(
    function(ev) {
        if (ev.ctrlKey) {
            const tooltips = document.body.getElementsByClassName("tooltip-inner");
            for (const e of tooltips) {
                if (!toggle)
                    e.textContent = "Go to crop";
                else
                    e.textContent = "Ignore this crop";
            }
        }
    }
)
$(document).keyup(
    function(ev) {
        if (!ev.ctrlKey) {
            const tooltips = document.body.getElementsByClassName("tooltip-inner");
            for (const e of tooltips) {
                if (!toggle)
                    e.textContent = "Ignore this crop";
                else
                    e.textContent = "Go to crop";
            }
        }
    }
)