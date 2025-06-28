export const tableClickColor = (id) => {
    document.querySelectorAll(id + ' tr').forEach(element=>
    {        
        element.addEventListener('click', () => {
            document.querySelectorAll(id + ' tbody tr:nth-child(odd)').forEach(tr => {
                if(element == tr)
                    element.style.backgroundColor ="#6CC2CC";
                else
                    tr.style.backgroundColor ="#ffffff";
            });
             document.querySelectorAll(id + ' tbody tr:nth-child(even)').forEach(tr => {
                if(element == tr)
                    element.style.backgroundColor ="#6CC2CC";
                else
                    tr.style.backgroundColor ="#f3f4f5";
            });  
        });        
    });
    
    document.querySelector(id + ' tbody tr').click();
}

export const initPageId = (page) =>
{
    let listPage = [];
    let startId = (page.pageId - 6) <= 0 ? 1 : (page.pageId - 6);

    var endId = startId + 6;
    if (endId > page.totalPage)
    {
        endId = page.totalPage;
        if ((endId - 6) > 0)
          startId = endId - 6; 
    }

    for (var i = startId; i <= endId; i++)
    {
        listPage.push(i);
    }

    return listPage;
}