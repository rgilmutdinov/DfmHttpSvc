<template>
    <div class="notify" :style="{ 'max-width': options.maxWidth, top: options.top, right: options.right }">
        <div v-for="(item, key) in items" :key="key">
            <div :class="item.options.itemClass" role="alert" style="padding-right: 4rem; padding-left: 3rem;">
                <span v-if="item.options.iconClass" class="icon" :class="item.options.iconClass" />
                <div v-if="item.options.mode === 'html'" v-html="item.text" />
                <template v-else>
                    {{ item.text }}
                </template>
                <button v-if="item.options.dismissable"
                        type="button"
                        class="close close-btn"
                        aria-label="Close"
                        @click="removeItem(key)">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .notify {
        opacity: 0.9;
        position: fixed;
        z-index: 9999;
    }

    .close-btn {
        position: absolute;
        top: 0;
        right: 0;
        padding: .75rem 1.25rem;
    }

    .icon {
        position: absolute;
        line-height: unset;
        top: 0;
        left: 0;
        padding: .75rem 1.25rem;
    }
</style>
<script>
    import Vue from 'vue';

    export default {
        data() {
            return {
                types: {
                    info: { itemClass: 'alert-info', iconClass: 'fas fa-info-circle' },
                    error: { itemClass: 'alert-danger', iconClass: 'fas fa-exclamation-triangl' },
                    warning: { itemClass: 'alert-warning', iconClass: 'fas fa-exclamation-circle' },
                    success: { itemClass: 'alert-success', iconClass: 'far fa-check-circle' }
                },
                options: {
                    itemClass: 'alert',
                    duration: 500,
                    visibility: 2000,
                    maxWidth: '400px',
                    top: '5px',
                    right: '15px',
                    mode: 'text',
                    permanent: false,
                    dismissable: true
                },
                items: {},
                idx: 0
            };
        },
        methods: {
            setTypes(types) {
                this.types = types;
            },
            addItem(type, msg, options) {
                let defaultOptions = {
                    iconClass: this.types[type].iconClass,
                    itemClass: [this.options.itemClass, this.types[type].itemClass],
                    visibility: this.options.visibility,
                    mode: this.options.mode,
                    permanent: this.options.permanent,
                    dismissable: this.options.dismissable
                };

                let itemOptions = Object.assign({}, defaultOptions, options);
                let idx = this.idx;

                // check if this message is already shown
                for (let key in this.items) {

                    if (this.items.hasOwnProperty(key)) {
                        if (this.items[key].text === msg) {
                            return;
                        }
                    }
                }

                // add it to the queue (if it's not already there)
                Vue.set(this.items, this.idx, { type: type, text: msg, options: itemOptions });

                this.idx++;

                // remove item if not permanent
                if (itemOptions.permanent === false) {
                    // remove item from array
                    setTimeout(() => { this.removeItem(idx); }, this.options.duration + itemOptions.visibility);
                }
            },
            removeItem(index) {
                Vue.delete(this.items, index);
            },
            removeAll() {
                this.items = {};
            }
        }
    };
</script>