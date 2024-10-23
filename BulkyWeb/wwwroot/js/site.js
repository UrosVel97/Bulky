$(document).ready(function () {
    //Ova funkcija kaze da elemenat ciji 'id' ima vrednost '#Input_Role' ce biti promenljen.
    $('#Input_Role').change(function () {
        //Ovde kaze da ako elemenat sa id-jem '#Input_Role' ima atribut 'selected', onda vrati tu vrednost koja je selektovana iz tela tog elementa
        var selection = $('#Input_Role Option:Selected').text();
        if (selection == 'Company') {
            //Elemenat ciji je id '#Input_CompanyId' prikazi u pogled
            $('#Input_CompanyId').show();
        }
        else {
            //Elemenat ciji je id '#Input_CompanyId' sakrij da se ne vidi u pogled
            $('#Input_CompanyId').hide();
        }
    })
})