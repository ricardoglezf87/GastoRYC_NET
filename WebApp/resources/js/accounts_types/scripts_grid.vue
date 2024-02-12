<template>
    <div>
        <table class="accounts_types-table">
            <thead>
                <tr>
                    <th>Descripción</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <input type="text" v-model="nuevaLinea.description">
                    </td>
                    <td>
                        <button @click="createAccountType"> <i class="fas fa-plus"></i> Nuevo estado</button>
                    </td>
                </tr>
                <tr v-for="account_type in accounts_types" :key="account_type.id">
                    <td>
                        <input type="text" v-model="account_type.description"
                            @keyup.enter="updateAccountType(account_type.id, 'description', account_type.description)"
                            @blur="updateAccountType(account_type.id, 'description', account_type.description)">
                    </td>
                    <td>
                        <button @click="deleteAccountType(account_type.id)">
                            <i class="fas fa-trash"></i>
                         </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
import axios from 'axios';
axios.defaults.headers.common['X-CSRF-TOKEN'] = document.querySelector('meta[name="csrf-token"]').content;

export default {
    data() {
        return {
            accounts_types: [],
            nuevaLinea: {
                description: '',
            },
        };
    },
    mounted() {
        this.loadAccountType();
    },
    methods: {
        updateAccountType(id, campo, valor) {
            axios
            .patch(`/api/accounts_types/${id}`, { campo, valor })
                .then(response => console.log('account_type actualizado:', response.data))
                .catch(error => console.error('Error updating account_type:', error.response.data));
        },
        createAccountType() {
            axios
                .post('/api/accounts_types', this.nuevaLinea)
                .then(response => {
                    console.log('account_type creado:', response.data);
                    // Limpiar la nueva línea después de crear el account_type
                    this.nuevaLinea = {
                        description: '',
                    };
                    // Recargar accounts_types después de la creación
                    this.loadAccountType();
                })
                .catch(error => console.error('Error creating account_type:', error.response.data));
        },
        loadAccountType() {
            axios
                .get('/api/accounts_types')
                .then(response => (this.accounts_types = response.data))
                .catch(error => console.error('Error loading accounts_types:', error.response.data));
        },
        deleteAccountType(id) {
            if (confirm("¿Estás seguro de que quieres borrar este account_type?")) {
                console.log(`id=${id}`);
                axios.delete(`/api/accounts_types/${id}`)
                    .then(() => {
                        console.log('account_type borrado con éxito');
                        this.loadAccountType(); // Recarga la lista después de borrar
                    })
                    .catch(error => console.error('Error al borrar account_type:', error.response.data));
            }
        }
    },
};
</script>

<style>
/* Estilos según sea necesario */
</style>
