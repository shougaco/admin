


document.addEventListener('keydown', function(event){

    if(event.keyCode == 191){
        const myModal = new bootstrap.Modal(document.getElementById("searchModal"), {});
        const myInput = document.getElementById('searchInput')
        const myModal2 = document.getElementById('searchModal');

        // check if myModal2 contains show class
        if(!myModal2.classList.contains('show')){
            myModal.show();
            myModal2.addEventListener('shown.bs.modal', () => {
                myInput.focus()
            })
        }
        
    }
} );