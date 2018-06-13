<template>
    <div :style="{ cursor, userSelect }" class="split-container" @mouseup="onMouseUp" @mousemove="onMouseMove">

        <div v-if="showLeft">
            <div class="panel panel-left" :style="styleLeft">
                <slot name="panel-left"></slot>
            </div>

            <div class="splitter" ref="splitter" :style="styleSplitter" @mousedown="onMouseDown" @click="onClick"></div>
        </div>

        <div class="panel panel-right" :style="styleRight">
            <slot name="panel-right"></slot>
        </div>

    </div>
</template>

<script>
    export default {
        name: 'SplitPanel',
        props: {
            sizeLeft: {
                type: Number,
                default: 200
            },
            maxSizeLeft: {
                type: Number,
                default: 300
            },
            minSizeLeft: {
                type: Number,
                default: 200
            },
            showLeft: {
                type: Boolean,
                default: false
            }
        },
        computed: {
            userSelect() {
                return this.active ? 'none' : ''
            },
            cursor() {
                return this.active ? 'col-resize' : ''
            },
            styleLeft() {
                return { 'width': this.size + 'px' }
            },
            styleSplitter() {
                return { 'left': this.size + 'px' }
            },
            styleRight() {
                if (this.showLeft) {
                    return { 'left': (this.size + this.splitterWidth) + 'px' }
                }
                return { 'left': '0px' }
            }
        },
        data() {
            return {
                active: false,
                hasMoved: false,
                size: 0,
                splitterWidth: 0
            }
        },
        mounted: function () {
            this.size = this.sizeLeft;
            this.splitterWidth = this.$refs.splitter.clientWidth;
        },
        methods: {
            onClick() {
                if (!this.hasMoved) {
                    this.$emit('resize');
                }
            },
            onMouseDown() {
                this.active = true;
                this.hasMoved = false;
            },
            onMouseUp() {
                this.active = false;
            },
            onMouseMove(e) {
                if (e.buttons === 0 || e.which === 0) {
                    this.active = false;
                }

                if (this.active) {
                    let offset = 0;
                    let target = e.currentTarget;

                    while (target) {
                        offset += target.offsetLeft;
                        target = target.offsetParent;
                    }

                    let newSize = e.pageX - offset;
                    newSize = Math.max(newSize, this.minSizeLeft);
                    newSize = Math.min(newSize, this.maxSizeLeft);
                    this.size = newSize;

                    this.$emit('resize');
                    this.hasMoved = true;
                }
            }
        }
    }
</script>

<style scoped>
    .split-container {
        height: 100%;
        position: relative;
    }

    .splitter {
        box-sizing: border-box;
        background: #000;
        position: absolute;
        opacity: .2;
        z-index: 1;
        background-clip: padding-box;
        width: 11px;
        height: 100%;
        margin-left: -5px;
        border-left: 5px solid white;
        border-right: 5px solid white;
        cursor: col-resize;
    }

    .panel {
        position: absolute;
        height: 100%;
        display: flex;
    }

    .panel-left {
        left: 0px;
        padding-right: 3px;
    }

    .panel-right {
        right: 0px;
        padding-left: 3px;
    }
</style>