$(document).ready(function () {
    $("body").tooltip({selector: '[data-toggle=tooltip]'});
});

$('th').click(function(){
    const table = $(this).parents('table').eq(0);
    let rows = table.find('tr:gt(0)').toArray().sort(comparer($(this).index()));
    this.asc = !this.asc
    if (!this.asc){
        rows = rows.reverse()
    }
    for (let i = 0; i < rows.length; i++){
        table.append(rows[i])
    }
})
function comparer(index) {
    return function(a, b) {
        const valA = getCellValue(a, index), valB = getCellValue(b, index);
        return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().toLowerCase().localeCompare(valB.toString().toLowerCase())
    }
}
function getCellValue(row, index) {
    return $(row).children('td').eq(index).text()
}