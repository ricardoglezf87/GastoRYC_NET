<template>
    <div>
        <section>
            <o-table :loading="isLoading"
                :data="transactions.current_page && transactions.data.lenght == 0 ? [] : transactions.data">
                <o-table-column field="id" label="ID" numeric v-slot="t">
                    {{ t.row.id }}
                </o-table-column>
                <o-table-column field="date" label="Fecha" date v-slot="t">
                    {{ t.row.date }}
                </o-table-column>
                <o-table-column field="id" label="Cuenta" numeric v-slot="t">
                    {{ t.row.accountid }}
                </o-table-column>
                <o-table-column field="id" label="Persona" numeric v-slot="t">
                    {{ t.row.personid }}
                </o-table-column>
                <o-table-column field="id" label="Categoria" numeric v-slot="t">
                    {{ t.row.categoryid }}
                </o-table-column>

            </o-table>
            <o-pagination v-if="transactions.current_page" v-model:current="currentPage" :total="transactions.total"
                :range-before="5" :range-after="5" order="centered" :simple="false" :rounded="true"
                :per-page="transactions.per_page" icon-prev="chevron-left" icon-next="chevron-right" @change="updatePage" />
        </section>
    </div>
</template>

<script>
import axios from 'axios';
axios.defaults.headers.common['X-CSRF-TOKEN'] = document.querySelector('meta[name="csrf-token"]').content;

export default {
    data() {
        return {
            transactions: [],
            isLoading: true,
            currentPage: 1,
        };
    },
    async mounted() {
        this.listPage();
    },
    methods: {
        updatePage() {
            setTimeout(this.listPage, 100);
        },
        listPage() {
            this.isLoading = true;
            axios.get('/api/transactions?page=' + this.currentPage)
                .then(response => {
                    this.transactions = response.data;
                    this.isLoading = false;
                })
                .catch(error => console.error('Error loading transactions:', error.response?.data || error.message));
        }
    }
};
</script>

<style>
/* Estilos según sea necesario */
</style>
