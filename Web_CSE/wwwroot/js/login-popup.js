const loginBtn = document.querySelectorAll('.btn-login');
const modalClose = document.querySelector('.modal-close');
const modal = document.querySelector('.modal');
const modalContainer = document.querySelector('.modal-container');
const fullSizeModal = document.querySelector('.modal-full');
const loginCancel = document.querySelector('.login-cancel');
function showModal() {
    modal.classList.add('open');
}
function hideModal() {
    modal.classList.remove('open');
    modalContainer.classList.remove('full-screen');
}
function FullSize() {

    modalContainer.classList.toggle('full-screen');
}

for (const btn of loginBtn) {
    btn.addEventListener('click', showModal);
};

modalClose.addEventListener('click', hideModal);
loginCancel.addEventListener('click', hideModal); 
modal.addEventListener('click', hideModal);
modalContainer.addEventListener('click', function (e) {
    e.stopPropagation();
})

fullSizeModal.addEventListener('click', FullSize);
