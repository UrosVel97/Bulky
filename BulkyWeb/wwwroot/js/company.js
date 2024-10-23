
function SweetAlert(id) {
    Swal.fire({
        title: "Da li ste sigurni?",
        text: "Nećete moći da vratite na staro!",
        icon: "warning",
        showCancelButton: false,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Da, obriši!"
    }).then((result) => {
        if (result.isConfirmed) {
            /*Pravimo odgovarajuci ajax request za metodu koju treba da obrise odgovarajuci
              slog iz tabele 'Product' na osnovu zadatog id-ja. Ako je uspesno obrisan slog onda
              ce se refresh-ovati stranica*/
            $.ajax({
                url: '/admin/company/delete/' + id,
                type: 'DELETE',
                /* AJAX prvo pronalazi metodu na osnovu URL i tipa metode. Zatim kada se metoda izvrsi
                   i ako ima neku povratnu vrednost, ta povratna vrednost se cuva u AJAX promenljivoj 'data'. */
                success: function (data) {

                    //Refresh-ujemo stranicu ako je uspesno obrisan slog iz tabele 'Product'
                    location.reload(true);

                },
                error: function (error) {
                    console.log(`Error ${error}`);
                }
            })
        }
    });
}

